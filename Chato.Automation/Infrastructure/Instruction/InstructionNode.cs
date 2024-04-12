﻿namespace Chato.Automation.Infrastructure.Instruction;

public class InstructionNodeInfo
{
    public string UserName { get; set; }
    public string Instruction { get; set; }
    public string Message { get; set; }
    public string? FromArrived { get; set; }
}

public class InstructionNode
{
    public InstructionNode(string userName , string instruction, string message , string ? fromArrived)
    {
        Children = new HashSet<InstructionNode>();
        UserName = userName;
        Instruction = instruction;
        Message = message;
        FromArrived = fromArrived;
    }

    public HashSet<InstructionNode> Children { get; }
    public string UserName { get; }
    public string Instruction { get; }
    public string Message { get; }
    public string? FromArrived { get; }
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

    public static InstructionNode Connect(this InstructionNode source, InstructionNode target)
    {
        source.Children.Add(target);
        return target;
    }
}