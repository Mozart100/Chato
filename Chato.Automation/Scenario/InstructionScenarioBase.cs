using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using System.Data.Common;

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
    }

    protected async Task InitializeAsync(params string[] users)
    {
        _counterSignal = new CounterSignal(users.Length - 1);

        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(user, BaseUrl, Logger, _counterSignal, isExpectingReciecingMessage: true);
            await executer.InitializeAsync();

            _users.Add(user, executer);
        }
    }

    public async Task InitializeWithGroupAsync(string groupName, params string[] users)
    {
        _counterSignal = new CounterSignal(users.Length - 1);

        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(user, BaseUrl, Logger, _counterSignal, isExpectingReciecingMessage: true);
            await executer.InitializeWithGroupAsync(groupName);

            _users.Add(user, executer);
        }

    }

    private async Task SendMessage(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, string message)
    {
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
                var userExecuter = _users[instruction.UserName];
                if (instruction.Instruction.Equals(UserHubInstruction.Publish_Instrauction))
                {
                    await _counterSignal.SetThrasholdAsync(instruction.Children.Count());
                    await SendMessage(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

                    if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
                    {
                        throw new Exception("Not all users receved their messages");
                    }
                }
                else
                {
                    if (instruction.Instruction.Equals(UserHubInstruction.Received_Instrauction))
                    {
                        await userExecuter.ListenCheck(instruction.FromArrived, instruction.Message);
                    }
                    else
                    {
                        if (instruction.Instruction.Equals(UserHubInstruction.Not_Received_Instrauction))
                        {
                            await userExecuter.NotReceivedCheck();
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