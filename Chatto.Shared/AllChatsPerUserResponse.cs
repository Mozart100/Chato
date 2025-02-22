using Chatto.Shared;

public record ChatInfoPerUser(string ChatName, ChatType ChatType, int AmountOfMembers,bool IsOwner);

public record AllChatsPerUserResponse(IEnumerable<ChatInfoPerUser> Chats);

