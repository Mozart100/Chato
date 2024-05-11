using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Arkovean.Chat.Automation.Scenario;

internal class PopulateDataScenario : InstructionScenarioBase
{
    private const string Haifa_Room = "haifa";

    private const string haifa_user1 = $"{Haifa_Room}_user1";
    private const string haifa_user2 = $"{Haifa_Room}_user2";
    private const string haifa_user3 = $"{Haifa_Room}_user3";
    
    public PopulateDataScenario(ILogger<PopulateDataScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        //SetupsLogicCallback.Add(TwoUserSetups);
        //BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);

        //BusinessLogicCallbacks.Add(() => GroupUsersCleanup(First_Group));
        //BusinessLogicCallbacks.Add(TreeUserSetups);
        //BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
        //BusinessLogicCallbacks.Add(() => GroupUsersCleanup(First_Group, Second_Group));

    }


    public override string ScenarioName => "Populatin real data";

    public override string Description => "Populating real data so client can use.";


    public async Task PopulateUsers()
    {

    }

}
