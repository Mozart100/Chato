using System.Text;

namespace Chato.Automation.Infrastructure.Instruction;

public record InstructionNode(string UserName, string? GroupName, UserInstractionBase Instruction, byte[] Message, string? FromArrived,
    HashSet<InstructionNode> Children)
{
    public InstructionNode(string userName, string? groupName, UserInstractionBase instruction, byte[] message, string? fromArrived)
        : this(userName, groupName, instruction, message, fromArrived, new HashSet<InstructionNode>())
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

    public static InstructionNode StartWithGroup(string groupName, byte[] message)
    {
        var info = new InstructionNode(userName: null, groupName: groupName, instruction: null, message: message, fromArrived: null);
        return info;
    }

    public static InstructionNode SendingTo(this InstructionNode info, string userName, string toPerson)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = new UserPeerToPeerInstruction() { Tag = toPerson },
            FromArrived = null,
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode ReceivingFrom(this InstructionNode info, string userName, string arrivedFrom)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = new UserReceivedInstruction(),
            FromArrived = arrivedFrom,
            Children = new(),

        };

        return @new;
    }

    //public static InstructionNode GetGroupInfo(this InstructionNode info,  string groupName)
    //{
    //    var @new = info with
    //    {
    //        UserName = info.UserName,
    //        Instruction = new GetRoomInfoInstruction(),
    //        FromArrived = null,
    //        Children = new(),
    //    };

    //    var result = Connect(info, @new);
    //    return result;
    //}



    internal static InstructionNode ReceivedVerification(this InstructionNode info, string userName, params InstructionNode[] fromInstructions)
    {
        var current = info;
        foreach (var instruction in fromInstructions)
        {
            var @new = info with
            {
                UserName = userName,
                Instruction = new UserReceivedInstruction(),
                FromArrived = instruction.UserName,
                Children = new(),
                GroupName = instruction.GroupName,
                Message = instruction.Message
            };

            current = current.Connect(@new);
        }

        return current;
    }

    public static InstructionNode IsToDownload(this InstructionNode info, string userName, byte[] data)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = new UserDownloadInstruction(),
            FromArrived = UserInstructionExecuter.Hub_From_Server,
            Children = new(),
            Message = data

        };

        return @new;
    }



    public static InstructionNode SendingBroadcast(this InstructionNode info, string userName)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = new UserBroadcastInstruction(),
            Children = new(),

        };

        return @new;
    }

    public static InstructionNode Do(this InstructionNode info, InstructionNode target, Func<InstructionNode, Task> operation)
    {
        var @new = info with
        {
            UserName = target.UserName,
            Instruction = new UserRunOperationInstruction() { Tag = operation },
            FromArrived = null,
            Message = null,
            Children = new(),
        };

        var result = Connect(info, @new);
        return result;
    }

    public static InstructionNode Is_Not_Received(this InstructionNode info, string userName)
    {
        var @new = info with
        {
            UserName = userName,
            Instruction = new UserNotReceivedInstruction(),
            Message = null,
            FromArrived = "none",
            Children = new(),
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