using System.Text.Json.Serialization;

namespace Chatto.Shared;

public class GetRoomResponse
{
    [JsonPropertyName("room")]

    public ChatRoomDto Chat { get; set; }
}

public record SenderInfo(string UserName, string Message);


public class ChatRoomDto
{
    [JsonPropertyName("roomName")]
    public string RoomName { get; init; }

    [JsonPropertyName("messages")]
    public SenderInfo[] Messages { get; set; }

    [JsonPropertyName("users")]
    public string[] Users { get; init; }

    [JsonConstructor]
    public ChatRoomDto(string roomName, SenderInfo[] messages, string[] users)
    {
        RoomName = roomName;
        Messages = messages;
        Users = users;
    }

    public static ChatRoomDto Empty() => new ChatRoomDto("", Array.Empty<SenderInfo>(), Array.Empty<string>());

    public override int GetHashCode() => RoomName.GetHashCode();

    public override bool Equals(object? obj) => obj is ChatRoomDto room && RoomName.Equals(room.RoomName);
}




