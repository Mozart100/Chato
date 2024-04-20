namespace Chato.Automation.Infrastructure.Instruction;

public class InstructionNodeInfo
{
    public string UserName { get; set; }
    public string? GroupName { get; set; }
    public string Instruction { get; set; }
    public string Message { get; set; }
    public string? FromArrived { get; set; }
}

public class InstructionNode : InstructionNodeInfo
{
    public InstructionNode(string userName , string instruction, string message , string ? fromArrived, string ? groupName =null)
    {
        Children = new HashSet<InstructionNode>();
        UserName = userName;
        Instruction = instruction;
        Message = message;
        FromArrived = fromArrived;

        GroupName = groupName;
    }

    public HashSet<InstructionNode> Children { get; }
}


public static class InstructionNodeFluentApi
{
    public static InstructionNodeInfo Start(string name)
    {
        var info = new InstructionNodeInfo { UserName = name };
        return info;
    }

    public static InstructionNode Send(this InstructionNodeInfo info, string message)
    {
        info.Message = message;
        return new InstructionNode(info.UserName, UserHubInstruction.Publish_Instrauction, message, null);
    }

    public static InstructionNode Receive(this InstructionNodeInfo info, string receiveFrom, string message)
    {
        info.Message = message;
        info.FromArrived = receiveFrom;
        return new InstructionNode(info.UserName, UserHubInstruction.Received_Instrauction, message, info.FromArrived);
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