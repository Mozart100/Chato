namespace Chato.Automation.Infrastructure.Instruction;

public static class UserInstructions
{

    public const string Received_Instruction = "received";//done
    public const string Not_Received_Instruction = "not_received";//done
    public const string Publish_Broadcasting_Instruction = "publish"; // done
    public const string Publish_PeerToPeer_Instruction = "peer_to_peer";//done
    public const string Publish_ToRestRoom_Instruction = "Rest_Room";//done

    public const string Run_Operation_Instruction = "do_operation";//done
    public const string Run_Download_Instruction = "do_download";
    //public const string Get_Group_Info_Instruction = "Get_Room_Info";
}


public abstract class UserInstractionBase
{
    public abstract string InstructionName { get; }
    public object Tag { get; set; } = null;

    public virtual async Task Execute(Func<Task> callback)
    {
        await callback?.Invoke();
    }
}

public class UserSendStringMessageRestRoomInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Publish_ToRestRoom_Instruction;
}


public class UserBroadcastInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Publish_Broadcasting_Instruction;
}

public class UserPeerToPeerInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Publish_PeerToPeer_Instruction;
}

public class UserReceivedInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Received_Instruction;
}

public class UserNotReceivedInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Not_Received_Instruction;
}


public class UserRunOperationInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Run_Operation_Instruction;
}

public class UserDownloadInstruction : UserInstractionBase
{
    public override string InstructionName => UserInstructions.Run_Download_Instruction;
}

//public class GetRoomInfoInstruction : UserInstractionBase
//{
//    public override string InstractionName => UserInstructions.Get_Group_Info_Instrauction;
//}

