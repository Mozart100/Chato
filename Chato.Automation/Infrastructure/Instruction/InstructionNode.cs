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
    public InstructionNode(string userName, string? groupName, string instruction, string message, string? fromArrived)
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
    public static InstructionNodeInfo Start(string name, string groupName = null)
    {
        var info = new InstructionNodeInfo
        {
            UserName = name,
            GroupName = groupName
        };

        return info;
    }

    public static InstructionNode ReplicateNameAndGroup(this InstructionNodeInfo info)
    {
        return new InstructionNode(userName: info.UserName, groupName: info.GroupName, instruction: null, message: null, fromArrived: null);
    }

    public static InstructionNode Send(this InstructionNodeInfo info, string message)
    {
        info.Message = message;
        return new InstructionNode(userName: info.UserName, groupName: null, instruction: UserHubInstruction.Publish_Instrauction, message: message, fromArrived: null);
    }

    public static InstructionNode Receive(this InstructionNodeInfo info, string receiveFrom, string message)
    {
        info.Message = message;
        info.FromArrived = receiveFrom;
        return new InstructionNode(userName: info.UserName, groupName: null, instruction: UserHubInstruction.Received_Instrauction, message: message, fromArrived: info.FromArrived);
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