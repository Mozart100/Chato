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
        info.Message = message;
        info.Instruction = UserHubInstruction.Publish_Instrauction;
        return info;
    }

    public static InstructionNode Receive(this InstructionNode info, string receiveFrom, string message)
    {
        info.Message = message;
        info.FromArrived = receiveFrom;
        info.Instruction = UserHubInstruction.Received_Instrauction;
        return info;
        //return new InstructionNode(userName: info.UserName, groupName: null, instruction: UserHubInstruction.Received_Instrauction, message: message, fromArrived: info.FromArrived);
    }

    //public static InstructionNode Not_Receive(this InstructionNodeInfo info, string receiveFrom, string message)
    //{
    //    info.Message = message;
    //    info.FromArrived = receiveFrom;
    //    return new InstructionNode(userName: info.UserName, groupName: null, instruction: UserHubInstruction.Received_Instrauction, message: message, fromArrived: info.FromArrived);
    //}

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