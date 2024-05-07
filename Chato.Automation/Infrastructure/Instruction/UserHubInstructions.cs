namespace Chato.Automation.Infrastructure.Instruction;

public static class UserHubInstructions
{

    public const string Received_Instrauction = "received";//done
    public const string Not_Received_Instrauction = "not_received";//done
    public const string Publish_Broadcasting_Instrauction = "publish"; // done
    public const string Publish_PeerToPeer_Instrauction = "peer_to_peer";//done

    public const string Run_Operation_Instrauction = "do_operation";//done
    public const string Run_Download_Instrauction = "do_download";
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
    public override string InstractionName => UserHubInstructions.Publish_Broadcasting_Instrauction;
}

public class UserPeerToPeerInstruction : UserInstractionBase
{
    public override string InstractionName => UserHubInstructions.Publish_PeerToPeer_Instrauction;
}

public class UserReceivedInstruction : UserInstractionBase
{
    public override string InstractionName => UserHubInstructions.Received_Instrauction;
}

public class UserNotReceivedInstruction : UserInstractionBase
{
    public override string InstractionName => UserHubInstructions.Not_Received_Instrauction;
}


public class UserRunOperationInstruction : UserInstractionBase
{
    public override string InstractionName => UserHubInstructions.Run_Operation_Instrauction;
}

public class UserDownloadInstruction : UserInstractionBase
{
    public override string InstractionName => UserHubInstructions.Run_Download_Instrauction;
}







