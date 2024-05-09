using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Responses;
using Chato.Server;
using Chato.Server.Controllers;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Text;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ScenarioBase
{
    private readonly CounterSignal _counterSignal;
    private readonly Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>> _actionMapper;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<string, UserInstructionExecuter> _users;
    private readonly Dictionary<string, HashSet<string>> _groupUsers;

    public InstructionScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        _users = new Dictionary<string, UserInstructionExecuter>();
        _groupUsers = new Dictionary<string, HashSet<string>>();

        _counterSignal = new CounterSignal(2);

        _actionMapper = new Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>>();

        HubUrl = $"{BaseUrl}/chat";
        AuthControllerUrl = $"{BaseUrl}/api/auth";
        RegisterAuthControllerUrl = $"{AuthControllerUrl}/register";
        LoginAuthControllerUrl = $"{AuthControllerUrl}/login";

        Initialize();

    }

    protected string HubUrl { get; }
    protected string AuthControllerUrl { get; }
    protected string RegisterAuthControllerUrl { get; }
    protected string LoginAuthControllerUrl { get; }

    public async Task RegisterUsers(params string[] users)
    {

        foreach (var user in users)
        {
            var registrationRequest = new RegisterAndLoginRequest { Password = "string", Username = user };
            var registrationInfo = await RunPostCommand<RegisterAndLoginRequest, RegisterResponse>(RegisterAuthControllerUrl, registrationRequest);
            var tokenResponse = await RunPostCommand<RegisterAndLoginRequest, LoginResponse>(LoginAuthControllerUrl, registrationRequest);

            var executer = new UserInstructionExecuter(registrationInfo, tokenResponse, HubUrl, Logger, _counterSignal);
            await executer.RegisterAsync();

            _users.Add(user, executer);
        }
    }

    public async Task AssignUserToGroupAsync(string groupName, params string[] users)
    {
        //var registrationRequest = new RegisterAndLoginRequest { Password = "string", Username = "string" };
        //var registrationInfo = await RunPostCommand<RegisterAndLoginRequest, RegisterResponse>(RegisterAuthControllerUrl, registrationRequest);
        //var tokenResponse = await RunPostCommand<RegisterAndLoginRequest, LoginResponse>(LoginAuthControllerUrl, registrationRequest);

        var stacked = default(HashSet<string>);
        if (_groupUsers.TryGetValue(groupName, out stacked) == false)
        {
            _groupUsers[groupName] = stacked ??= new HashSet<string>();
        }


        foreach (var user in users)
        {
            var executer = _users[user]; ;// new UserInstructionExecuter(registrationInfo, tokenResponse, HubUrl, Logger, _counterSignal);
            await executer.InitializeWithGroupAsync(groupName);

            stacked.Add(user);
        }
    }


    private async Task SendBroadcastingMessage(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        if (groupName == null)
        {
            await userExecuter.SendMessageToAllUsers(userNameFrom: userNameFrom, message: message);
        }
        else
        {
            await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, message: message);
        }
    }


    private async Task SendPeerToPeerMessage(UserInstructionExecuter userExecuter, string userNameFrom, string toUser, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.SendMessageFromUserToUserUsers(userNameFrom: userNameFrom, toUser: toUser, message: message);
    }

    private void Initialize()
    {

        _actionMapper.Add(UserInstructions.Publish_Broadcasting_Instrauction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstractionName != UserInstructions.Not_Received_Instrauction).Count());
            await SendBroadcastingMessage(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users receved their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Publish_PeerToPeer_Instrauction, async (userExecuter, instruction) =>
        {
            var toUser = instruction.Instruction.Tag as string ?? throw new ArgumentNullException("Should be user name");

            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstractionName != UserInstructions.Not_Received_Instrauction).Count());
            await SendPeerToPeerMessage(userExecuter: userExecuter, userNameFrom: instruction.UserName, toUser: toUser, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users receved their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Received_Instrauction, async (userExecuter, instruction) => await userExecuter.ListenToStringCheckAsync(instruction.FromArrived, instruction.Message));
        _actionMapper.Add(UserInstructions.Not_Received_Instrauction, async (userExecuter, instruction) => await userExecuter.NotReceivedCheckAsync());
        _actionMapper.Add(UserInstructions.Run_Download_Instrauction, async (userExecuter, instruction) => await userExecuter.DownloadStream(instruction.Message));

        //_actionMapper.Add(UserHubInstructions.Run_Verify_Instrauction, async (userExecuter, instruction) => await userExecuter.VerifyHistoryAsync(new Queue<byte[]>()));
    }


    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.MoveNext();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.InstractionName.Equals(UserInstructions.Run_Operation_Instrauction))
                {
                    if (instruction.Instruction.Tag is Func<InstructionNode, Task> callback)
                    {
                        await callback(instruction);
                    }

                    continue;
                }

                var userExecuter = _users[instruction.UserName];
                await _actionMapper[instruction.Instruction.InstractionName]?.Invoke(userExecuter, instruction);
            }

            instructions = await graph.MoveNext();
        }
    }

    protected async Task UsersCleanup()
    {
        foreach (var user in _users.Values)
        {
            await user.Close();
        }

        _users.Clear();
    }

    public async Task UsersCleanup(params string[] users)
    {
        foreach (var user in users)
        {
            foreach (var group in _groupUsers.Keys.ToArray())
            {
                if (_groupUsers[group].Contains(user))
                {
                    _groupUsers[group].Remove(user);
                    if (_groupUsers[group].Any() == false)
                    {
                        _groupUsers[group] = null;
                        _groupUsers.Remove(group);
                    }
                }
            }

            await _users[user].UserDisconnectingAsync();
            await _users[user].KillConnectionAsync();
            _users.Remove(user);
        }
    }

    public async Task GroupUsersCleanup(params string[] groupNames)
    {
        foreach (var groupName in groupNames)
        {
            foreach (var user in _groupUsers[groupName].ToArray())
            {
                await _users[user].GroupClose(groupName);
            }
        }

        foreach (var user in _users.Values)
        {
            await user.KillConnectionAsync();
        }

        _users.Clear();
        _groupUsers.Clear();
    }
}