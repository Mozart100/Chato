using Chato.Automation.Infrastructure.Instruction;
using Chatto.Shared;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ChatoRawDataScenarioBase
{
    protected readonly CounterSignal _counterSignal;
    private readonly Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>> _actionMapper;
    private readonly CancellationTokenSource _cancellationTokenSource;

    protected readonly Dictionary<string, UserInstructionExecuter> Users;
    private readonly Dictionary<string, HashSet<string>> _groupUsers;

    public InstructionScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        Users = new Dictionary<string, UserInstructionExecuter>();
        _groupUsers = new Dictionary<string, HashSet<string>>();

        _counterSignal = new CounterSignal(2);

        _actionMapper = new Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>>();



        Initialize();

    }

    public virtual async Task RegisterUsers(params string[] users)
    {
        foreach (var user in users)
        {
            var registrationRequest = new RegistrationRequest { UserName = user, Age = 20, Description = $"Description_{user}", Gender = "male" };
            var registrationInfo = await RunPostCommand<RegistrationRequest, RegistrationResponse>(RegisterAuthControllerUrl, registrationRequest);

            var executer = new UserInstructionExecuter(registrationInfo, HubUrl, Logger, _counterSignal);
            await executer.RegisterAsync();

            Users.Add(user, executer);
        }
    }

    public async Task AssignUserToGroupAsync(string groupName, params string[] users)
    {
        var stacked = default(HashSet<string>);
        if (_groupUsers.TryGetValue(groupName, out stacked) == false)
        {
            _groupUsers[groupName] = stacked ??= new HashSet<string>();
        }


        foreach (var user in users)
        {
            var executer = Users[user]; ;// new UserInstructionExecuter(registrationInfo, tokenResponse, HubUrl, Logger, _counterSignal);
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
            await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, ptr: message);
        }
    }


    private async Task SendPeerToPeerMessage(UserInstructionExecuter userExecuter, string userNameFrom, string toUser, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.SendMessageFromUserToUserUsers(userNameFrom: userNameFrom, toUser: toUser, ptr: message);
    }

    private async Task SendMessageToOthersInGroup(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, ptr: message);

    }



    private void Initialize()
    {

        _actionMapper.Add(UserInstructions.Publish_Broadcasting_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstructionName != UserInstructions.Not_Received_Instruction).Count());
            await SendBroadcastingMessage(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Publish_PeerToPeer_Instruction, async (userExecuter, instruction) =>
        {
            var toUser = instruction.Instruction.Tag as string ?? throw new ArgumentNullException("Should be user name");

            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstructionName != UserInstructions.Not_Received_Instruction).Count());
            await SendPeerToPeerMessage(userExecuter: userExecuter, userNameFrom: instruction.UserName, toUser: toUser, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }
        });

        _actionMapper.Add(UserInstructions.Publish_ToRestRoom_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstructionName != UserInstructions.Not_Received_Instruction).Count());
            await SendMessageToOthersInGroup(userExecuter: userExecuter, groupName:instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Received_Instruction, async (userExecuter, instruction) => await userExecuter.ListenToStringCheckAsync(instruction.FromArrived, instruction.Message));
        _actionMapper.Add(UserInstructions.Not_Received_Instruction, async (userExecuter, instruction) => await userExecuter.NotReceivedCheckAsync());
        _actionMapper.Add(UserInstructions.Run_Download_Instruction, async (userExecuter, instruction) => await userExecuter.DownloadStream(instruction.Message));
        _actionMapper.Add(UserInstructions.Leave_Room_Instruction, async (userExecuter, instruction) => await userExecuter.LeaveGroupInfo(instruction.GroupName));
    }


    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.MoveNext();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.InstructionName.Equals(UserInstructions.Run_Operation_Instruction))
                {
                    if (instruction.Instruction.Tag is Func<InstructionNode, Task> callback)
                    {
                        await callback(instruction);
                    }

                    continue;
                }

                var userExecuter = Users[instruction.UserName];
                await _actionMapper[instruction.Instruction.InstructionName]?.Invoke(userExecuter, instruction);
            }

            instructions = await graph.MoveNext();
        }
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

            await Users[user].UserDisconnectingAsync();
            await Users[user].KillConnectionAsync();
            Users.Remove(user);
        }
    }

}