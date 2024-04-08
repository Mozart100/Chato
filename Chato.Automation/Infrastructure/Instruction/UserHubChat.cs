using Chato.Automation.Scenario;

namespace Chato.Automation.Infrastructure.Instruction;

public record InstructionDetail(string Command, string From);


public class UserHubInstruction
{
    private const string Received_Instrauction = "received";
    private const string Publish_Instrauction = "publish";
    private const string Wait_Instrauction = "publish";

    private readonly Queue<string> _instruction;

    public UserHubInstruction()
    {
        _instruction = new Queue<string>();
    }

    public void AddRecieveInstruction(string from = "server")
    {
        _instruction.Enqueue($"{Received_Instrauction};{from};");
    }

    public void AddWaitInstruction(string from = "server")
    {
        _instruction.Enqueue($"{Wait_Instrauction};{from};");
    }

    public void AddPublishInstruction(string from = "server")
    {
        _instruction.Enqueue($"{Publish_Instrauction};{from};");
    }

    public bool InstructionAny => _instruction.Any();



    public InstructionDetail GetInstruction()
    {
        var command = _instruction.Dequeue();
        var @arguments = command.Split(";");

        return new InstructionDetail(@arguments[0], @arguments[1]);
    }
}


public class UserHubChat
{
    private readonly IHubConnector _hubConnection;
    private readonly UserHubInstruction _instruction;

    public UserHubChat(IHubConnector hubConnection, string name)
    {
        _hubConnection = hubConnection;
        Name = name;

        _instruction = new UserHubInstruction();
    }

    public bool IsSuccessful => !_instruction.InstructionAny;

    public string Name { get; }


    public void AddRecieveInstruction()
    {
        _instruction.AddRecieveInstruction();
    }

    public void AddWaitInstruction()
    {
        _instruction.AddWaitInstruction();
    }

    public void AddPublishInstruction()
    {
        _instruction.AddPublishInstruction();
    }

    public Task SendAsync(string message)
    {
        return _hubConnection.SendMessageToAllUSers(Name, message);
    }

    public void Received(string arrivedFrom, string message)
    {

        _hubConnection.Logger.Info($"{Name} - received from [{arrivedFrom}] [{message}]");
        var instruction = _instruction.GetInstruction();

        if (!instruction.From.Equals(arrivedFrom))
        {
            throw new Exception("xxx");
        }

        //if(_instruction.InstructionAny == false)
        //{

        //}
    }
}
