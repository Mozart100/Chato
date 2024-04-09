using Chato.Automation.Scenario;
using FluentAssertions;

namespace Chato.Automation.Infrastructure.Instruction;

public record InstructionDetail(string Command, string From,string Message);


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

    public void AddRecieveInstruction(string message , string from = "server")
    {
        _instruction.Enqueue($"{Received_Instrauction};{from};{message}");
    }

    public void AddWaitInstruction(string from = "server")
    {
        _instruction.Enqueue($"{Wait_Instrauction};{from};");
    }

    public void AddPublishInstruction(string from ,string messge)
    {
        _instruction.Enqueue($"{Publish_Instrauction};{from};{messge}");
    }

    public bool InstructionAny => _instruction.Any();



    public InstructionDetail GetInstruction()
    {
        var command = _instruction.Dequeue();
        var @arguments = command.Split(";");

        return new InstructionDetail(@arguments[0], @arguments[1], arguments[2]);
    }
}


public class UserHubChat
{
    private readonly IHubConnector _hubConnection;
    private readonly UserHubInstruction _instructions;

    public UserHubChat(IHubConnector hubConnection, string name)
    {
        _hubConnection = hubConnection;
        Name = name;

        _instructions = new UserHubInstruction();
    }

    public override bool Equals(object? obj)
    {
        if ( obj != null && obj is UserHubChat user)
        {
            return Name.Equals(user.Name);  
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }


    public bool IsSuccessful => !_instructions.InstructionAny;

    public string Name { get; }


    public void AddRecieveInstruction(string message)
    {
        _instructions.AddRecieveInstruction(message);
    }

    public void AddWaitInstruction()
    {
        _instructions.AddWaitInstruction();
    }

    public void AddPublishInstruction(string message)
    {
        _instructions.AddPublishInstruction(Name,message);
    }

    public Task SendAsync(string message)
    {
        return _hubConnection.SendMessageToAllUSers(Name, message);
    }

    public bool ReceivedAndCheck(string arrivedFrom, string message)
    {

        _hubConnection.Logger.Info($"{Name} - received from [{arrivedFrom}] [{message}]");
        var instruction = _instructions.GetInstruction();


        instruction.From.Should().Be(arrivedFrom);
        instruction.Message.Should().Be(message);

        return _instructions.InstructionAny == false;
    }
}
