using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Arkovean.Chat.Automation.Scenario;

internal class BasicScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";
    private const string Second_Group = "nesher";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";

    public BasicScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        BusinessLogicCallbacks.Add(VerificationSetups);
        BusinessLogicCallbacks.Add(VerificationStep);
        SummaryLogicCallback.Add(() => GroupUsersCleanup(First_Group));

    }


    public override string ScenarioName => "All sort of mini scenarios";

    public override string Description => "To run mini scenarios.";



    private async Task VerificationStep()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.IsSender(Anatoliy_User);
        var olessyaSender = firstGroup.IsSender(Olessya_User);
        var maxReceiver1 = firstGroup.IsReciever(Max_User, olessyaSender.UserName);

        anatoliySender.Connect(olessyaSender)
            .Do(maxReceiver1, async user => await AssignUserToGroupAsync(First_Group, Max_User))
            .Verificationn(Max_User, anatoliySender, olessyaSender);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }


    private async Task VerificationSetups()
    {
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User);
    }

}
