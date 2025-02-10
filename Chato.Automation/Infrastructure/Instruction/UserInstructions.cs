using Chato.Server.DataAccess.Models;
using Chatto.Shared;

namespace Chato.Automation.Infrastructure.Instruction;

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
    public const string Notify_User_Instruction = "notify_users";



    public const string logout_Chat_Instruction = "logout";
    public const string JoinOrCreate_Chat_Instruction = "join_or_create_chat";
    public const string GetHistory_Chat_Instruction = "get_history_chat";
}




public abstract class UserInstructionBase
{
    public abstract string InstructionName { get; }
    public object Tag { get; set; } = null;

}

public class UserSendStringMessageRestRoomInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Publish_ToRestRoom_Instruction;

    public int AmountAwaits { get; init; } = 1;

}


//public class UserBroadcastInstruction : UserInstructionBase
//{
//    public override string InstructionName => UserInstructions.Publish_Broadcasting_Instruction;
//}

public class UserRegisterLobiInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.User_RegisterLobi_Instruction;

    public int AmountMessages { get; set; } = -1;

    //public ChatType ChatType { get; set; } = ChatType.Public;
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

    public Func<IUserInfo, Task> Operation { get; set; }
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
    //public record JoinOrCreateRecod(int amountMessages);kc

    public override string InstructionName => UserInstructions.JoinOrCreate_Chat_Instruction;

    public int AmountMessages { get; set; } = -1;

    public ChatType ChatType { get; set; } = ChatType.Public;

    public SenderInfoType? ExpectedSenderInfoType { get; set; }
}

public class GetHistoryChatInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.GetHistory_Chat_Instruction;
    public int AmountMessages { get; set; } = -1;
}

public class NotifyUserInstruction : UserInstructionBase
{
    public override string InstructionName => UserInstructions.Notify_User_Instruction;
}






