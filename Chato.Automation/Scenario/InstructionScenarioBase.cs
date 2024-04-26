using Chato.Automation.Infrastructure.Instruction;
using System.Collections;
using System.Text;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ScenarioBase
{
    private CounterSignal _counterSignal;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Dictionary<string, UserInstructionExecuter> _users;

    public InstructionScenarioBase(string baseUrl) : base(baseUrl)
    {
        _users = new Dictionary<string, UserInstructionExecuter>();
        SummaryLogicCallback.Add(UsersCleanup);
        _counterSignal = new CounterSignal(2);

    }

    protected async Task InitializeAsync(params string[] users)
    {
        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(user, BaseUrl, Logger, _counterSignal);
            await executer.InitializeAsync();

            _users.Add(user, executer);
        }
    }

    public async Task InitializeWithGroupAsync(string groupName, params string[] users)
    {
        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(user, BaseUrl, Logger, _counterSignal);
            await executer.InitializeWithGroupAsync(groupName);

            _users.Add(user, executer);
        }
    }

    private async Task SendStringMessage(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, byte [] ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        if (groupName == null)
        {
            await userExecuter.SendMessageToAllUSers(userNameFrom: userNameFrom, message: message);
        }
        else
        {
            await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, message: message);
        }
    }

    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.MoveNext();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.Equals(UserHubInstructions.Run_Operation_Instrauction))
                {
                    await instruction.Operation(null);
                    continue;
                }
                

                var userExecuter = _users[instruction.UserName];
                if (instruction.Instruction.Equals(UserHubInstructions.Publish_Instrauction))
                {
                    await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x=>x.Instruction != UserHubInstructions.Not_Received_Instrauction).Count());
                    await SendStringMessage(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, ptr: instruction.Message);

                    if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
                    {
                        throw new Exception("Not all users receved their messages");
                    }
                }
                else
                {
                    if (instruction.Instruction.Equals(UserHubInstructions.Received_Instrauction))
                    {
                        await userExecuter.ListenStringCheck(instruction.FromArrived, instruction.Message);
                    }
                    else
                    {
                        if (instruction.Instruction.Equals(UserHubInstructions.Not_Received_Instrauction))
                        {
                            await userExecuter.NotReceivedCheck();
                        }
                        else
                        {
                            if (instruction.Instruction.Equals(UserHubInstructions.Run_Download_Instrauction))
                            {
                                await userExecuter.DownloadStream();
                                //await userExecuter.NotReceivedCheck();k
                            }
                        }
                    }
                }
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

    protected async Task GroupUsersCleanup(string groupName)
    {
        foreach (var user in _users.Values)
        {
            await user.GroupClose(groupName);
        }

        _users.Clear();
    }
}