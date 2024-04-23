namespace Chato.Automation.Infrastructure.Instruction;

public record InstructionNode(string UserName, string? GroupName, string Instruction, string Message, string? FromArrived, HashSet<InstructionNode> Children)
{
    public InstructionNode(string userName, string? groupName, string instruction, string message, string? fromArrived)
        : this(userName, groupName, instruction, message, fromArrived, new HashSet<InstructionNode>())
    {
    }
}

public static class InstructionNodeFluentApi
{
    public static InstructionNode StartWithGroup(string groupName , string message)
    {
        var info = new InstructionNode(userName: null, groupName: groupName, instruction: null, message: message, fromArrived: null);
        return info;
    }

    public static InstructionNode IsReciever(this InstructionNode info , string userName, string arrivedFrom)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstruction.Received_Instrauction,
            FromArrived = arrivedFrom,
            Children = new()
            
        };

        return @new;
    }

    public static InstructionNode IsSender(this InstructionNode info, string userName)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstruction.Publish_Instrauction,
            Children = new()

        };

        return @new;
    }



    public static InstructionNode Start(string name, string groupName = null)
    {
        var info = new InstructionNode(userName: name, groupName: groupName, instruction: null, message: null, fromArrived: null);
        return info;
    }

    public static InstructionNode ReplicateNameAndGroup(this InstructionNode info)
    {
        return new InstructionNode(userName: info.UserName, groupName: info.GroupName, instruction: null, message: null, fromArrived: null);
    }

    public static InstructionNode Send(this InstructionNode info, string message)
    {
        var @new = info with
        {
            Message = message,
            Instruction = UserHubInstruction.Publish_Instrauction,
            Children = new()
        };

        return @new;
    }

    public static InstructionNode Receive(this InstructionNode info, string receiveFrom, string message)
    {
        var @new = info with
        {
            Message = message,
            Instruction = UserHubInstruction.Received_Instrauction,
            FromArrived = receiveFrom,
            Children = new()
        };

        return @new;
    }

    public static InstructionNode Not_Receive(this InstructionNode info)
    {
        var @new = info with
        {
            Message = null,
            FromArrived = "none",
            Children = new()
        };

        return @new;
    }

    public static InstructionNode Connect(this InstructionNode source, params InstructionNode[] targets)
    {
        InstructionNode node = null;
        foreach (var target in targets)
        {
            source.Children.Add(target);
            node = target;
        }
        return node;
    }
}