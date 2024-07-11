using Chato.Server.DataAccess.Models;
using Chatto.Shared;
using System.Text.Json.Serialization;

//public class GetAllRoomRequest
//{

//}

public class GetAllRoomResponse
{

    [JsonPropertyName("rooms")]
    public ChatRoomDto[] Rooms { get; set; }
}
