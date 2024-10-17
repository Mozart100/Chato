
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.DataAccess.Models;


public class ChatDb : EntityDbBase
{
    public override string Id
    {
        get => RoomName;
        set => RoomName = value;
    }
    public string RoomName { get; private set; }

    public List<SenderInfo> Messages { get; set; } = new List<SenderInfo>();
    public HashSet<string> Users { get;  } = new HashSet<string>();

}


public static class ChatRoomDbExtensions
{
    public static  ChatRoomDto ToChatRoomDto(this ChatDb chatRoomDb)
    {
        return new ChatRoomDto(chatRoomDb.RoomName, chatRoomDb.Users.SafeToArray());
    }


    public static SenderInfo AddMessage(this ChatDb chatRoom, SenderInfoType senderInfoType, string fromUser, string? textMessage, string? image)
    {
        var senderInfo = default(SenderInfo);

        if (chatRoom is not null)
        {
            senderInfo =  new SenderInfo(senderInfoType, fromUser, textMessage,  image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            chatRoom.Messages.Add(senderInfo);
        };

        return senderInfo;
    }

    public static bool ContainUser(this ChatDb chatRoom, string user)
    {
        if (chatRoom is not null)
        {
            foreach (var item in chatRoom.Users)
            {
                if(item.Equals(user,StringComparison.OrdinalIgnoreCase))
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

