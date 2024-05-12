
namespace Chato.Server.DataAccess.Models;

public record SenderInfo(string UserName,byte[] Message);

public class ChatRoomDb : EntityDbBase
{
    public override string Id
    {
        get => GroupName;
        set => GroupName = value;
    }
    public string GroupName { get; private set; }

    public List<SenderInfo> SenderInfo { get; set; } = new List<SenderInfo>();
    public HashSet<string> Users { get;  } = new HashSet<string>();

}

