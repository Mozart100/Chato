
using Chato.Server.Infrastracture;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chato.Server.DataAccess.Models;

public record SenderInfo(string UserName,byte[] Message);

public class ChatRoomDb : EntityDbBase
{
    public override string Id
    {
        get => RoomName;
        set => RoomName = value;
    }
    public string RoomName { get; private set; }

    public List<SenderInfo> SenderInfo { get; set; } = new List<SenderInfo>();
    public HashSet<string> Users { get;  } = new HashSet<string>();

}



public struct ChatRoomDto
{
    public string RoomName { get; init; }

    public SenderInfo[] SenderInfo { get; set; }
    public string[] Users { get; init; }


    public ChatRoomDto(string roomName, SenderInfo[] senderInfos, string[] users)
    {
        RoomName = roomName;
        SenderInfo = senderInfos;
        Users = users;
    }

    public static ChatRoomDto Empty() => new ChatRoomDto("", null, null);

    public override int GetHashCode() => RoomName.GetHashCode();

    public override bool Equals(object? obj) => obj is ChatRoomDb room && RoomName.Equals(room.Id);
}


public static class ChatRoomDbExtensions
{
    public static  ChatRoomDto ToChatRoomDto(this ChatRoomDb chatRoomDb)
    {
        return new ChatRoomDto(chatRoomDb.RoomName, chatRoomDb.SenderInfo.SafeToArray(), chatRoomDb.Users.SafeToArray());
    }
}

