namespace Chato.Automation.Infrastructure.Instruction;

//public record InstructionDetail(string Command, string From, string Message);

public class UserHubInstruction
{

    public const string Received_Instrauction = "received";
    public const string Not_Received_Instrauction = "not_received";
    public const string Publish_Instrauction = "publish";
    public const string Ru_Operation_Instrauction = "join_room";

    //private readonly Queue<string> _instruction;

    //public UserHubInstruction()
    //{
    //    _instruction = new Queue<string>();
    //}

    //public string Peek() => _instruction.Peek();

    //public void AddRecieveInstruction(string message, string from = "server")
    //{
    //    _instruction.Enqueue($"{Received_Instrauction};{from};{message}");
    //}

    //public void AddJoinRoomInstruction(string from = "server")
    //{
    //    _instruction.Enqueue($"{Join_Room_Instrauction};{from};");
    //}

    //public void AddPublishInstruction(string from, string messge)
    //{
    //    _instruction.Enqueue($"{Publish_Instrauction};{from};{messge}");
    //}

    //public bool InstructionAny => _instruction.Any();



    //public InstructionDetail GetInstruction()
    //{
    //    var command = _instruction.Dequeue();
    //    var @arguments = command.Split(";");

    //    return new InstructionDetail(@arguments[0], @arguments[1], arguments[2]);
    //}
}
