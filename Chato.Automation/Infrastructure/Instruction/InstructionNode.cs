using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chato.Automation.Infrastructure.Instruction;

public record InstructionNode(string UserName, string? GroupName, string Instruction, byte [] Message, string? FromArrived,
    HashSet<InstructionNode> Children, Func<InstructionNode, Task>? Operation = null)
{
    public InstructionNode(string userName, string? groupName, string instruction, byte [] message, string? fromArrived)
        : this(userName, groupName, instruction, message, fromArrived, new HashSet<InstructionNode>(), null)
    {
    }

   
}



public static class InstructionNodeFluentApi
{
    public static InstructionNode StartWithGroup(string groupName, string message)
    {
        var byteArray = Encoding.UTF8.GetBytes(message);
        var info = new InstructionNode(userName: null, groupName: groupName, instruction: null, message: byteArray, fromArrived: null);
        return info;
    }

    public static InstructionNode StartWithGroup(string groupName, byte [] message)
    {
        var info = new InstructionNode(userName: null, groupName: groupName, instruction: null, message: message, fromArrived: null);
        return info;
    }

    public static InstructionNode RecievingFrom(this InstructionNode info, string userName, string arrivedFrom)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstructions.Received_Instrauction,
            FromArrived = arrivedFrom,
            Children = new(),
            Operation = null

        };

        return @new;
    }

    internal static InstructionNode Verificationn(this InstructionNode info, string userName,params InstructionNode[] fromInstructions)
    {
        var current = info;
        foreach (var instruction in fromInstructions)
        {
            var @new = info with
            {
                UserName = userName,
                Instruction = UserHubInstructions.Received_Instrauction,
                FromArrived = instruction.UserName,
                Children = new(),
                Operation = null,
                GroupName = instruction.GroupName,
                Message = instruction.Message
            };

            current = current.Connect(@new);
        }

        return current;
    }

    public static InstructionNode IsToDownload(this InstructionNode info, string userName, byte[] data )
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstructions.Run_Download_Instrauction,
            FromArrived = UserInstructionExecuter.Hub_From_Server,
            Children = new(),
            Operation = null,
            Message = data

        };

        return @new;
    }



    public static InstructionNode SendingBroadcast(this InstructionNode info, string userName)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstructions.Publish_Instrauction,
            Children = new(),
            Operation = null

        };

        return @new;
    }

    public static InstructionNode Do(this InstructionNode info, InstructionNode target, Func<InstructionNode, Task> operation)
    {
        var @new = info with
        {
            UserName = target.UserName,
            Instruction = UserHubInstructions.Run_Operation_Instrauction,
            FromArrived = null,
            Message = null,
            Children = new(),
            Operation = operation

        };

        var result = Connect(info, @new);
        return result;
    }

    public static InstructionNode Is_Not_Receiver(this InstructionNode info, string userName)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = UserHubInstructions.Not_Received_Instrauction,
            Message = null,
            FromArrived = "none",
            Children = new(),
            Operation = null
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