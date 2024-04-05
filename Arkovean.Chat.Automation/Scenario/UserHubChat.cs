namespace Arkovean.Chat.Automation.Scenario;

public class UserHubInstruction
{
    private const string Received_Instrauction = "received"; 
    private const string Publish_Instrauction = "publish";
    
    private readonly Queue<string> _instruction;

    public UserHubInstruction()
    {
        _instruction = new Queue<string>();

        Initialize();
    }

    private void Initialize()
    {
        _instruction.Enqueue($"{Received_Instrauction};server;");
    }

    public (string instruction,string from) GetInstruction()
    {
        var command = _instruction.Dequeue();
        var  @arguments= command.Split(";");

        return (@arguments[0], @arguments[1]);
    }
}

public class UserHubChat
{
    private readonly HubScenarioBase _hubConnection;
    private readonly Dictionary<string, Action<string, string>> _actions;
    private readonly UserHubInstruction _instruction;

    public UserHubChat(HubScenarioBase hubConnection, string name)
    {
        this._hubConnection = hubConnection;
        Name = name;

        _instruction = new UserHubInstruction();


        _actions = new Dictionary<string, Action<string,string>>();
    }

    public string Name { get; }


    public Task SendAsync(string message)
    {
        return _hubConnection.SendMessageToAllUSers(Name, message);
    }

    public void Received(string arrivedFrom, string message)
    {
        _hubConnection.Logger.Info($"{Name} - received from [{arrivedFrom}] [{message}]");

        var (instruction,from) = _instruction.GetInstruction();


        _actions[instruction](arrivedFrom, message);
    }

}
