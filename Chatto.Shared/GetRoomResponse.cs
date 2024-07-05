using System.Text.Json.Serialization;

namespace Chatto.Shared;

public class GetRoomResponse
{
    [JsonPropertyName("room")]

    public ChatRoomDto Room { get; set; }
}

public record SenderInfo(string UserName, byte[] Message);


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




