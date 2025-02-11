﻿using Chatto.Shared;

public record ChatInfoPerUser(string ChatName, ChatType ChatType, int AmountOfMembers);

public record AllChatsPerUserResponse(IEnumerable<ChatInfoPerUser> Chats);

