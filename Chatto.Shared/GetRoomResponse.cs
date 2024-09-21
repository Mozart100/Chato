using System.Text.Json.Serialization;

namespace Chatto.Shared;

public class GetRoomResponse
{
    [JsonPropertyName("room")]

    public ChatRoomDto Chat { get; set; }
}

public record SenderInfo(string UserName, string Message);
public record HistoryMessageInfo(string ChatName, string UserName, string Message) : SenderInfo(UserName, Message);

public class ChatRoomDto
{
    [JsonPropertyName("chatName")]
    public string ChatName { get; init; }

    [JsonPropertyName("messages")]
    public SenderInfo[] Messages { get; set; }

    [JsonPropertyName("users")]
    public string[] Users { get; init; }

    [JsonConstructor]
    public ChatRoomDto(string chatName, SenderInfo[] messages, string[] users)
    {
        ChatName = chatName;
        Messages = messages;
        Users = users;
    }

    public static ChatRoomDto Empty() => new ChatRoomDto("", Array.Empty<SenderInfo>(), Array.Empty<string>());

    public override int GetHashCode() => ChatName.GetHashCode();

    public override bool Equals(object? obj) => obj is ChatRoomDto room && ChatName.Equals(room.ChatName);
}




