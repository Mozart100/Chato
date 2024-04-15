using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ScenarioBase
{
    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    //protected SemaphoreSlim FinishedSignal;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Dictionary<string, UserInstructionExecuter> _users;

    public InstructionScenarioBase(string baseUrl) : base(baseUrl)
    {
        _users = new Dictionary<string, UserInstructionExecuter>();
        SummaryLogicCallback.Add(UsersCleanup);

    }

    protected async Task InitializeAsync(params string[] users)
    {
        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(user, BaseUrl, Logger);
            await executer.InitializeAsync();

            _users.Add(user, executer);
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
                    await userExecuter.SendMessageToAllUSers(userNameFrom: instruction.UserName, message: instruction.Message);
                    await Task.Delay(4000);
                }
                else
                {
                    if (instruction.Instruction.Equals(UserHubInstruction.Received_Instrauction))
                    {
                        await userExecuter.ListenCheck(instruction.FromArrived, instruction.Message);
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
    }
}
