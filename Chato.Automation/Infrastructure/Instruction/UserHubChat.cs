using AutoMapper.Internal.Mappers;
using Chato.Automation.Scenario;

namespace Chato.Automation.Infrastructure.Instruction;

public class UserHubInstruction
{
    private const string Received_Instrauction = "received";
    private const string Publish_Instrauction = "publish";

    private readonly Queue<string> _instruction;

    public UserHubInstruction()
    {
        _instruction = new Queue<string>();

        //Initialize();
    }

    public void AddRecieveInstruction(string from ="server")
    {
        _instruction.Enqueue($"{Received_Instrauction};{from};");
    }

    public bool InstructionAny => _instruction.Any();

    private void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            _instruction.Enqueue($"{Received_Instrauction};server;");
        }
    }

    public (string instruction, string from) GetInstruction()
    {
        var command = _instruction.Dequeue();
        var @arguments = command.Split(";");

        return (@arguments[0], @arguments[1]);
    }
}

public class UserHubChatArgs : EventArgs
{

}

public class UserHubChat
{
    private readonly IHubConnector _hubConnection;
    private readonly Dictionary<string, Action<string, string>> _actions;
    private readonly UserHubInstruction _instruction;

    public UserHubChat(IHubConnector hubConnection, string name)
    {
        _hubConnection = hubConnection;
        Name = name;

        _instruction = new UserHubInstruction();


        _actions = new Dictionary<string, Action<string, string>>();
    }

    public bool IsSuccessful => _instruction.InstructionAny;

    public event EventHandler<UserHubChatArgs> Finished;

    public string Name { get; }


    public void AddRecieveInstruction()
    {
        _instruction.AddRecieveInstruction();
    }


    public Task SendAsync(string message)
    {
        return _hubConnection.SendMessageToAllUSers(Name, message);
    }

    public void Received(string arrivedFrom, string message)
    {
        _hubConnection.Logger.Info($"{Name} - received from [{arrivedFrom}] [{message}]");
        var (instruction,from) = _instruction.GetInstruction();



        if (  Finished != null)
        {
            if(_instruction.InstructionAny == false)
            {
                Finished(this, new UserHubChatArgs { });
            }
        }


        //var (instruction,from) = _instruction.GetInstruction();


        //_actions[instruction](arrivedFrom, message);
    }

}
