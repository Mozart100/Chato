using Chato.Server.DataAccess.Models;
using Chatto.Shared;
using System.Text.Json.Serialization;



public class GetAllRoomResponse
{

    [JsonPropertyName("rooms")]
    public ChatDto[] Rooms { get; set; }
}
