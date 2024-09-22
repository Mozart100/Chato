using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Text.Json.Serialization;

namespace Chatto.Shared;

public class GetRoomResponse
{
    [JsonPropertyName("room")]

    public ChatRoomDto Chat { get; set; }
}

public record SenderInfo(string UserName, byte [] Message,bool IsText);
public record HistoryMessageInfo(string ChatName, string UserName, string Message);

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




