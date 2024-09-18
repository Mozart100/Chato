using Chato.Server.Services;
using Chatto.Shared;
using System.Text;

namespace Chato.Automation.Infrastructure.Instruction;

public interface IUserInfo
{
    public InstructionNode Instruction { get; }
    public RegistrationResponse RegistrationResponse { get; }
}


public record UserInfo(InstructionNode Instruction, RegistrationResponse RegistrationResponse) : IUserInfo;

public record InstructionNode(string UserName, string? ChatName, UserInstructionBase Instruction, byte[] Message, string? FromArrived,
    HashSet<InstructionNode> Children)
{
    public InstructionNode(string userName, string? groupName, UserInstructionBase instruction, byte[] message, string? fromArrived)
        : this(userName, groupName, instruction, message, fromArrived, new HashSet<InstructionNode>())
    {
    }
}

public static class InstructionNodeFluentApi
{
    public static Dictionary<string, InstructionNode> RegisterInLoLobi(params string[] users)
    {
        var instructions = new Dictionary<string, InstructionNode>();
        foreach (var user in users)
        {
            var info = new InstructionNode(userName: user, groupName: IChatService.Lobi, instruction: new UserRegisterLobiInstruction(), message: null, fromArrived: null);
            instructions[user] = info;
        }

        return instructions;
    }

    public static InstructionNode ReceivingMessage(this InstructionNode info, string chatName, string arrivedFrom, string message)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new UserReceivedInstruction(),
            FromArrived = arrivedFrom,
            Message = Encoding.UTF8.GetBytes(message),
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode Logout(this InstructionNode info)
    {
        var @new = info with
        {
            FromArrived = null,
            Instruction = new LogoutInstruction(),
            Message = null,
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode JoinOrCreateChat(this InstructionNode info, string chatName)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new JoinOrCreateChatInstruction(),
            FromArrived = null,
            Message = null,
            Children = new(),
        };

        return @new;
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

    public static InstructionNode SendingToRestRoom(this InstructionNode info, string message, string chatName,int amountAwait2)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new UserSendStringMessageRestRoomInstruction { AmountAwaits = amountAwait2 },
            Message = Encoding.UTF8.GetBytes(message),
            Children = new(),

        };

        return @new;
    }

    public static InstructionNode LeaveRoom(this InstructionNode info, string chat)
    {
        var @new = info with
        {
            ChatName = chat,
            Instruction = new LeaveRoomInstruction(),
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode Do(this InstructionNode info, Func<IUserInfo, Task> operation)
    {
        var @new = info with
        {
            Instruction = new UserRunOperationInstruction() { Tag = operation },//parameter = token
            FromArrived = null,
            Message = null,
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode Is_Not_ReceivedMessage(this InstructionNode info, string chatame)
    {
        var @new = info with
        {
            ChatName = chatame,
            Instruction = new UserNotReceivedInstruction(),
            Message = null,
            FromArrived = "none",
            Children = new(),
        };

        return @new;
    }



    public static InstructionNode Step(this InstructionNode source, params InstructionNode[] targets)
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