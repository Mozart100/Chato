﻿namespace Chato.Automation.Infrastructure.Instruction;

public static class UserInstructions
{

    public const string Received_Instruction = "received";//done
    public const string Not_Received_Instruction = "not_received";//done
    //public const string Publish_Broadcasting_Instruction = "send_Everyone"; // done
    public const string Publish_PeerToPeer_Instruction = "peer_to_peer";//donekc
    public const string User_RegisterLobi_Instruction = "user_enter_lobi";//done
    public const string Publish_ToRestRoom_Instruction = "send_Rest_Room";//done

    public const string Run_Operation_Instruction = "do_operation";//done
    public const string Leave_Room_Instruction = "leave_room";


    public const string logout_Chat_Instruction = "logout";
    public const string JoinOrCreate_Chat_Instruction = "join_or_create_chat";
    public const string GetHistory_Chat_Instruction = "get_history_chat";
}


public abstract class UserInstructionBase
{
    public abstract string InstructionName { get; }
    public object Tag { get; set; } = null;
    public int AmountAwaits { get; init; } = 1;

    public virtual async Task Execute(Func<Task> callback)
    {
        await callback?.Invoke();
    }
}

public class UserSendStringMessageRestRoomInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Publish_ToRestRoom_Instruction;
}


//public class UserBroadcastInstruction : UserInstructionBase
//{
//    public override string InstructionName => UserInstructions.Publish_Broadcasting_Instruction;
//}

public class UserRegisterLobiInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.User_RegisterLobi_Instruction;
}


public class UserReceivedInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Received_Instruction;
}

public class UserNotReceivedInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Not_Received_Instruction;
}


public class UserRunOperationInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Run_Operation_Instruction;
}

public class LeaveRoomInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Leave_Room_Instruction;
}


public class LogoutInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.logout_Chat_Instruction;
}

public class JoinOrCreateChatInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.JoinOrCreate_Chat_Instruction;
}

public class GetHistoryChatInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.GetHistory_Chat_Instruction;
}
