//using Chato.Automation.Scenario;
//using FluentAssertions;
//using FluentAssertions.Specialized;

//namespace Chato.Automation.Infrastructure.Instruction;

//public abstract class UserChatBase
//{
//    private readonly IHubConnector _hubConnection;
//    private readonly UserHubInstruction _instructions;

//    public UserChatBase(IHubConnector hubConnection, string name)
//    {
//        _hubConnection = hubConnection;
//        Name = name;

//        _instructions = new UserHubInstruction();
//    }

//    public override bool Equals(object? obj)
//    {
//        if (obj != null && obj is UserHubChat user)
//        {
//            return Name.Equals(user.Name);
//        }

//        return false;
//    }

//    public override int GetHashCode()
//    {
//        return Name.GetHashCode();
//    }

//    public bool IsSuccessful => !_instructions.InstructionAny;

//    public string Name { get; }



//}

//public class UserHubChat
//{
//    private readonly IHubConnector _hubConnection;
//    private readonly UserHubInstruction _instructions;

//    public UserHubChat(IHubConnector hubConnection, string name)
//    {
//        _hubConnection = hubConnection;
//        Name = name;

//        _actions = new Dictionary<string, Func<string, Task>>();
//        _actions.Add(UserHubInstruction.Publish_Instrauction, SendAsync);
//        _instructions = new UserHubInstruction();
//    }

//    public override bool Equals(object? obj)
//    {
//        if (obj != null && obj is UserHubChat user)
//        {
//            return Name.Equals(user.Name);
//        }

//        return false;
//    }

//    public override int GetHashCode()
//    {
//        return Name.GetHashCode();
//    }


//    public bool IsSuccessful => !_instructions.InstructionAny;

//    public string Name { get; }

//    private readonly Dictionary<string, Func<string, Task>> _actions;

//    public void AddRecieveInstruction(string message, string from)
//    {
//        _instructions.AddRecieveInstruction(message, from);
//    }

//    public void AddWaitInstruction()
//    {
//        _instructions.AddWaitInstruction();
//    }

//    public void AddPublishInstruction(string message)
//    {
//        _instructions.AddPublishInstruction(Name, message);
//    }

//    public async Task ExecuteAsync()
//    {
//        var instruction = _instructions.GetInstruction();

//        if (instruction.Command.Equals(UserHubInstruction.Publish_Instrauction))
//        {
//            await SendAsync(instruction.Message);
//        }
//    }

//    public Task SendAsync(string message)
//    {
//        return _hubConnection.SendMessageToAllUSers(Name, message);
//    }

//    public bool ReceivedAndCheck(string arrivedFrom, string message)
//    {

//        _hubConnection.Logger.Info($"{Name} - received from [{arrivedFrom}] [{message}]");
//        var instruction = _instructions.GetInstruction();


//        instruction.From.Should().Be(arrivedFrom);
//        instruction.Message.Should().Be(message);

//        return _instructions.InstructionAny == false;
//    }
//}
