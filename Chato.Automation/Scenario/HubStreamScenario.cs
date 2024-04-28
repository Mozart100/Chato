using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class HubStreamScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";
    private byte[] _fileContent;

    public HubStreamScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(TwoUserSetups);
        //BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
        SummaryLogicCallback.Add(GroupUsersCleanup);

        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "css.txt");
        _fileContent = File.ReadAllBytes(path);

    }

    public override string ScenarioName => "Streaming messages";

    public override string Description => "Uploading and downloading messaging.";

    private async Task TwoUserSetups()
    {
        await InitializeWithGroupAsync(First_Group, Anatoliy_User, Olessya_User);
    }

    //private async Task TwoPeopleHandShakeStep()
    //{
    //    var message_1 = "Hello";
    //    var groupRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);

    //    var anatoliySender = groupRoot.IsToDownload(Anatoliy_User, _fileContent);
    //    var olessyaReceive = groupRoot.IsToDownload(Olessya_User, _fileContent);


    //    anatoliySender.Connect(olessyaReceive);

    //    var graph = new InstructionGraph(anatoliySender);
    //    await InstructionExecuter(graph);
    //}


    private async Task TreeUserSetups()
    {
        await InitializeWithGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);



        var anatoliySender = firstGroup.IsSender(Anatoliy_User);
        var olessyaReceive1 = firstGroup.IsReciever(Olessya_User, anatoliySender.UserName);
        var nathanReceive1 = firstGroup.IsReciever(Nathan_User, anatoliySender.UserName);

        anatoliySender.Connect(nathanReceive1, olessyaReceive1);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }



    public async Task GroupUsersCleanup()
    {
        await GroupUsersCleanup(First_Group);
    }

}
