namespace Chato.Server.DataAccess.Models;



public record SenderInfo(string UserName,byte[] Message);

public class ChatRoomDb : EntityDbBase
{
    public string City { get; set; }

    public List<SenderInfo> SenderInfo { get; set; } = new List<SenderInfo>();
}

