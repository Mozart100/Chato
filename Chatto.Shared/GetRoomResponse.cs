using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Text.Json.Serialization;

namespace Chatto.Shared;

public class GetRoomResponse
{
    [JsonPropertyName("room")]

    public ChatRoomDto Chat { get; set; }
}

public enum SenderInfoType:uint
{
    TextMessage = 1,
    Image = 2,
    Joined = 3
}

public record SenderInfo( SenderInfoType SenderInfoType, string FromUser, string ? TextMessage, string ? Image, long TimeStemp);
public record MessageInfo(SenderInfoType SenderInfoType,string ChatName, string FromUser, string? TextMessage, string? Image , long TimeStemp = 0) : SenderInfo( SenderInfoType, FromUser,TextMessage,Image,DateTimeOffset.UtcNow.ToUnixTimeSeconds());

public class ChatRoomDto
{
    [JsonPropertyName("chatName")]
    public string ChatName { get; init; }

    //[JsonPropertyName("messages")]
    //public SenderInfo[] Messages { get; set; }

    [JsonPropertyName("users")]
    public string[] Users { get; init; }

    [JsonConstructor]
    public ChatRoomDto(string chatName, string[] users)
    {
        ChatName = chatName;
        Users = users;
    }

    public static ChatRoomDto Empty() => new ChatRoomDto("", Array.Empty<string>());

    public override int GetHashCode() => ChatName.GetHashCode();

    public override bool Equals(object? obj) => obj is ChatRoomDto room && ChatName.Equals(room.ChatName);
}




