
using Chato.Server.Infrastracture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

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


public class ChatRoomDto
{
    [JsonPropertyName("roomName")]
    public string RoomName { get; init; }

    [JsonPropertyName("senderInfo")]
    public SenderInfo[] SenderInfo { get; set; }

    [JsonPropertyName("users")]
    public string[] Users { get; init; }

    [JsonConstructor]
    public ChatRoomDto(string roomName, SenderInfo[] senderInfo, string[] users)
    {
        RoomName = roomName;
        SenderInfo = senderInfo;
        Users = users;
    }

    public static ChatRoomDto Empty() => new ChatRoomDto("", Array.Empty<SenderInfo>(), Array.Empty<string>());

    public override int GetHashCode() => RoomName.GetHashCode();

    public override bool Equals(object? obj) => obj is ChatRoomDto room && RoomName.Equals(room.RoomName);
}


public static class ChatRoomDbExtensions
{
    public static  ChatRoomDto ToChatRoomDto(this ChatRoomDb chatRoomDb)
    {
        return new ChatRoomDto(chatRoomDb.RoomName, chatRoomDb.SenderInfo.SafeToArray(), chatRoomDb.Users.SafeToArray());
    }
}

