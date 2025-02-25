
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.DataAccess.Models;


public class Chat : EntityDbBase, IChatEnittyMapper
{
    public override string Id
    {
        get => RoomName;
        set => RoomName = value;
    }

    public ChatType ChatType { get; set; }
    public string RoomName { get; private set; }
    public string Description { get; set; }
    public required DateTime Expire { get; set; }
    public IEnumerable<string> Files => FileSegment.GetImages();


    public IEnumerable<SenderInfo> Messages => UserMessages;
    public IEnumerable<string> Users => ActiveUsers;

    //--------------------------------------------------------------------------------------

    public HashSet<string> ActiveUsers { get; } = new HashSet<string>();
    public List<SenderInfo> UserMessages { get; set; } = new List<SenderInfo>();
    public FilesSegment FileSegment { get; set; } = new FilesSegment();
}


public static class ChatRoomDbExtensions
{
    public static ChatRoomDto ToChatRoomDto(this Chat chatRoomDb)
    {
        return new ChatRoomDto(chatRoomDb.RoomName, chatRoomDb.Description, chatRoomDb.ActiveUsers.SafeToArray());
    }


    public static SenderInfo AddTextMessage(this Chat chatRoom, SenderInfoType senderInfoType, string fromUser, string? textMessage, string? image)
    {
        var senderInfo = default(SenderInfo);

        if (chatRoom is not null)
        {
            senderInfo = new SenderInfo(senderInfoType, fromUser, textMessage, image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            chatRoom.UserMessages.Add(senderInfo);
        };

        return senderInfo;
    }

    public static SenderInfo AddImageMessage(this Chat chatRoom, string fromUser, string? textMessage, string? image)
    {
        var senderInfo = default(SenderInfo);


        if (chatRoom is not null)
        {
            senderInfo = new SenderInfo(SenderInfoType.Image, fromUser, textMessage, image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            chatRoom.UserMessages.Add(senderInfo);
        };

        return senderInfo;
    }

    //public static (int AmoutMessages, SenderInfo SenderInfo) AddImageMessageold(this ChatDb chatRoom, string fromUser, string? textMessage, string? image)
    //{
    //    var senderInfo = default(SenderInfo);
    //    int amountMessage = -1;


    //    if (chatRoom is not null)
    //    {
    //        senderInfo = new SenderInfo(SenderInfoType.Image, fromUser, textMessage, image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    //        chatRoom.Messages.Add(senderInfo);

    //        amountMessage = chatRoom.Messages.Count;
    //    };

    //    return (amountMessage, senderInfo);
    //}

    public static bool ContainUser(this Chat chatRoom, string user)
    {
        if (chatRoom is not null)
        {
            foreach (var item in chatRoom.ActiveUsers)
            {
                if (item.Equals(user, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        };

        return false;
    }


    //public static bool RemoveUser(this ChatDb chatRoom, string user)
    //{
    //    if (chatRoom is not null)
    //    {
    //        foreach (var item in chatRoom.Users)
    //        {
    //            if (item.Equals(user, StringComparison.OrdinalIgnoreCase))
    //            {
    //                return true;
    //            }
    //        }
    //    };

    //    return false;
    //}


}

