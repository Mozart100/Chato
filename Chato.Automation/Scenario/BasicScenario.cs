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

        BusinessLogicCallbacks.Add(TreeUserSetups);
        BusinessLogicCallbacks.Add(TreePoepleStep);
        SummaryLogicCallback.Add(() => GroupUsersCleanup(First_Group));

    }


    public override string ScenarioName => "People sending Message";

    public override string Description => "Users sending and listening.";



    private async Task TreePoepleStep()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.IsSender(Anatoliy_User);
        var olessyaSender = firstGroup.IsSender(Olessya_User);
        var maxReceiver1 = firstGroup.IsReciever(Max_User, olessyaSender.UserName);

        anatoliySender.Connect(olessyaSender)
            .Do(maxReceiver1, async user => await InitializeWithGroupAsync(First_Group, Max_User))
            .Verificationn(Max_User, anatoliySender, olessyaSender);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }


    private async Task TreeUserSetups()
    {
        await InitializeWithGroupAsync(First_Group, Anatoliy_User, Olessya_User);
    }

}
