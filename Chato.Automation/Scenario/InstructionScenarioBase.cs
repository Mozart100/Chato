using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using Chato.Server.Services;
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
            var registrationInfo = await RunPostCommand<RegistrationRequest, ResponseWrapper<RegistrationResponse>>(RegisterAuthControllerUrl, registrationRequest);

            var executer = new UserInstructionExecuter(registrationInfo.Body, HubUrl, Logger, _counterSignal);
            //await executer.RegisterAsync();

            Users.Add(user, executer);
        }
    }

    private async Task StartSignalR(UserInstructionExecuter userExecuter)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.RegisterAsync2222();

    }

    private async Task SendMessageToOthersInGroup(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, ptr: message);

    }



    private void Initialize()
    {

        _actionMapper.Add(UserInstructions.logout_Chat_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(1);
            await userExecuter.UserDisconnectingAsync();


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }

            await UsersCleanup2222(userExecuter.UserName);
        });


        _actionMapper.Add(UserInstructions.JoinOrCreate_Chat_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(1);

            await userExecuter.JoinOrCreateChat2222(instruction.GroupName);


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }

        });




        _actionMapper.Add(UserInstructions.User_RegisterLobi_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(1);
            await StartSignalR(userExecuter: userExecuter);


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }


            await userExecuter.ListenToStringCheckAsync2222(IChatService.Lobi, instruction.UserName, "server", ChattoHub.User_Connected_Message);
        });




        _actionMapper.Add(UserInstructions.Publish_ToRestRoom_Instruction, async (userExecuter, instruction) =>
        {
            //await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction.InstructionName != UserInstructions.Not_Received_Instruction).Count());
            await _counterSignal.SetThrasholdAsync(instruction.Instruction.AmountAwaits);
            await SendMessageToOthersInGroup(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Received_Instruction, async (userExecuter, instruction) => await userExecuter.ListenToStringCheckAsync2222(instruction.GroupName, instruction.UserName, instruction.FromArrived, instruction.Message));
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
                    if (instruction.Instruction.Tag is Func<IUserInfo, Task> callback)
                    {
                        var registerInfo = default(RegistrationResponse);
                        if (Users.TryGetValue(instruction.UserName, out var ins))
                        {
                            registerInfo = ins.RegisterResponse;
                        }
                        await callback(new UserInfo(instruction, registerInfo));
                    }

                    continue;
                }

                var userExecuter = Users[instruction.UserName];
                await _actionMapper[instruction.Instruction.InstructionName]?.Invoke(userExecuter, instruction);
            }

            instructions = await graph.MoveNext();
        }
    }

    public async Task UsersCleanup2222(params string[] users)
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

            await Users[user].KillConnectionAsync();
            Users.Remove(user);
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