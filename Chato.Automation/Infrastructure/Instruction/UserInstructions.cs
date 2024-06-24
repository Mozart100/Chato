namespace Chato.Automation.Infrastructure.Instruction;

public static class UserInstructions
{

    public const string Received_Instrauction = "received";//done
    public const string Not_Received_Instrauction = "not_received";//done
    public const string Publish_Broadcasting_Instrauction = "publish"; // done
    public const string Publish_PeerToPeer_Instrauction = "peer_to_peer";//done

    public const string Run_Operation_Instrauction = "do_operation";//done
    public const string Run_Download_Instrauction = "do_download";
    public const string Get_Group_Info_Instrauction = "Get_Room_Info";
}


public abstract class UserInstractionBase
{
    public abstract string InstractionName { get; }
    public object Tag { get; set; } = null;

    public virtual async Task Execute(Func<Task> callback)
    {
        await callback?.Invoke();
    }
}

public class UserBroadcastInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Publish_Broadcasting_Instrauction;
}

public class UserPeerToPeerInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Publish_PeerToPeer_Instrauction;
}

public class UserReceivedInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Received_Instrauction;
}

public class UserNotReceivedInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Not_Received_Instrauction;
}


public class UserRunOperationInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Run_Operation_Instrauction;
}

public class UserDownloadInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Run_Download_Instrauction;
}

public class GetRoomInfoInstruction : UserInstractionBase
{
    public override string InstractionName => UserInstructions.Get_Group_Info_Instrauction;
}

