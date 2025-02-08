using Chatto.Shared;

public record ChatInfoPerUser(string ChatName, int AmountOfMembers);

public record AllChatsPerUserResponse(IEnumerable<ChatInfoPerUser> Chats);

