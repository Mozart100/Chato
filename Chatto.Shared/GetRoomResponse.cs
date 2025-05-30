﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Text.Json.Serialization;

namespace Chatto.Shared;



public enum ChatConnectionType
{
    Joined,
    Created,
    Denied
}



public record ChatNotification(string ChatName,ChatConnectionType ChatConnectionType);


public class GetRoomResponse
{
    public ChatDto Chat { get; set; }
}

public enum SenderInfoType : uint
{
    TextMessage = 1,
    Image = 2,
    Joined = 3,
    Created = 4
}

public record SenderInfo(SenderInfoType SenderInfoType, string FromUser, string? TextMessage, string? Image, long TimeStemp);
public record MessageInfo(SenderInfoType SenderInfoType, string ChatName, string FromUser, string? TextMessage, string? Image, long TimeStemp = 0, string? DescriptionOfChat = null)
    : SenderInfo(SenderInfoType, FromUser, TextMessage, Image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

public interface IChatEnittyMapper : IAutomapperEntities
{
    ChatType ChatType { get; set; }
    string RoomName { get; }

    IEnumerable<SenderInfo> Messages { get; }
    IEnumerable<string> Users { get; } 

    string Description { get; set; }

    DateTime Expire { get; set; }

    IEnumerable<string> Files { get;  }
}

public class ChatDto : IChatEnittyMapper
{
    public ChatType ChatType { get; set; }
    public string RoomName { get; set; }
    public IEnumerable<SenderInfo> Messages { get; set; }

    public IEnumerable<string> Users { get; set; }

    public string Description { get;set; }
    public DateTime Expire { get;set; }
    public IEnumerable<string> Files { get; set; }
}

//public class ChatRoomDto
//{
//    [JsonPropertyName("chatName")]
//    public string ChatName { get; init; }

//    [JsonPropertyName("description")]
//    public string Description { get; init; }
//    //[JsonPropertyName("messages")]
//    //public SenderInfo[] Messages { get; set; }

//    [JsonPropertyName("users")]
//    public string[] Users { get; init; }

//    [JsonConstructor]
//    public ChatRoomDto(string chatName, string description, string[] users)
//    {
//        ChatName = chatName;
//        Users = users;
//    }

//    public static ChatRoomDto Empty() => new ChatRoomDto("", string.Empty, Array.Empty<string>());

//    public override int GetHashCode() => ChatName.GetHashCode();

//    public override bool Equals(object? obj) => obj is ChatRoomDto room && ChatName.Equals(room.ChatName);
//}




