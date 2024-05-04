using Chato.Server.DataAccess.Models;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRoomRepository : IRepositoryBase<ChatRoomDb>
{
    Task AddSenderInfoAsync(string chatRoomId, SenderInfo senderInfo);
}

public class ChatRoomRepository : RepositoryBase<ChatRoomDb>, IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger)
    {
        _logger = logger;
    }

    public async Task AddSenderInfoAsync(string chatRoomId, SenderInfo senderInfo)
    {
        var chatRoom = await GetAsync(x => x.Id == chatRoomId);

        if(chatRoom is not null)
        {
            chatRoom.SenderInfo.Add(senderInfo);
        }
    }
}