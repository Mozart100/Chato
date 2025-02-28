﻿using Chato.Server.DataAccess.Models;
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

public record InstructionNode(string UserName, string? ChatName, UserInstructionBase Instruction, string Message, string? FromArrived, SenderInfoType messageType, string? ImageName,
    HashSet<InstructionNode> Children)
{
    public InstructionNode(string userName, string? groupName, UserInstructionBase instruction, string message, string? fromArrived, SenderInfoType messageType, string? imageName)
        : this(userName, groupName, instruction, message, fromArrived, messageType, imageName, new HashSet<InstructionNode>())
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
            var info = RegisterSingleUserInLoLobi(user);  // new InstructionNode(userName: user, groupName: IChatService.Lobi, instruction: new UserRegisterLobiInstruction(), message: null, fromArrived: null);
            instructions[user] = info;
        }

        return instructions;
    }

    public static InstructionNode RegisterSingleUserInLoLobi(string user)
    {
        return new InstructionNode(userName: user, groupName: IChatService.Lobi, instruction: new UserRegisterLobiInstruction() { AmountMessages = -1 }, message: null, fromArrived: null, messageType: SenderInfoType.TextMessage, null);
    }


    public static InstructionNode ReceivingMessage(this InstructionNode info, string chatName, string arrivedFrom, string message, SenderInfoType messageType = SenderInfoType.TextMessage, string imageName = null)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new UserReceivedInstruction(),
            FromArrived = arrivedFrom,
            Message = message,
            Children = new(),
            messageType = messageType,
            ImageName = imageName
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

    //public static InstructionNode NotifyUser(this InstructionNode info, string  chat)
    //{
    //    var @new = info with
    //    {
    //        ChatName = chat,
    //        FromArrived = null,
    //        Instruction = new NotifyUserInstruction(),
    //        Message = null,
    //        Children = new(),
    //    };

    //    return @new;
    //}

    public static InstructionNode JoinOrCreatePrivateChatxxx(this InstructionNode info, string chatName, int amountMessages = -1, SenderInfoType? senderInfoType = null, string? description = null)
    {
        return JoinOrCreateChat(info, chatName, ChatType.Private, amountMessages, senderInfoType, description);
    }

    public static InstructionNode JoinOrCreatePublicChat(this InstructionNode info, string chatName, int amountMessages = -1, SenderInfoType? senderInfoType = null, string? description = null)
    {

        return JoinOrCreateChat(info, chatName, ChatType.Public, amountMessages, senderInfoType, description);
    }

    private static InstructionNode JoinOrCreateChat(this InstructionNode info, string chatName, ChatType chatType, int amountMessages = -1, SenderInfoType? senderInfoType = null, string? description = null)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new JoinOrCreateChatInstruction() { AmountMessages = amountMessages, ChatType = chatType, ExpectedSenderInfoType = senderInfoType, Description = description },
            FromArrived = null,
            Message = null,
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode GetHistoryChat(this InstructionNode info, string chatName, int amountMessages = -1)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new GetHistoryChatInstruction() { AmountMessages = amountMessages },
            FromArrived = null,
            Message = null,
            Children = new(),
        };

        return @new;
    }

    public static InstructionNode SendingTextToRestRoom(this InstructionNode info, string message, string chatName, int amountAwait,
        SenderInfoType messageType = SenderInfoType.TextMessage, string imageName = null)
    {
        var @new = info with
        {
            ChatName = chatName,
            Instruction = new UserSendStringMessageRestRoomInstruction { AmountAwaits = amountAwait },
            Message = message,
            Children = new(),
            messageType = messageType,
            ImageName = imageName
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
            Instruction = new UserRunOperationInstruction() { Operation = operation },//parameter = token
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