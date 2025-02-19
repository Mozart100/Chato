using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Chatto.Shared;
using System;
using System.Reflection;
using System.Text;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRepository : IRepositoryBase<ChatDb>
{
   Task<bool> UpdateImagesAsync(Predicate<ChatDb> selector, IEnumerable<string> images);
}

public class ChatRepository : RepositoryBase<ChatDb>, IChatRepository
{
    private readonly ILogger<ChatRepository> _logger;
    private readonly IRoomIndexerRepository _roomIndexerRepository;

    public ChatRepository(ILogger<ChatRepository> logger, IRoomIndexerRepository roomIndexerRepository)
    {
        _logger = logger;
        this._roomIndexerRepository = roomIndexerRepository;
    }

    public override IEnumerable<ChatDb> GetAll()
    {
        var rooms = base.GetAll();
        foreach (var item in rooms)
        {
            _roomIndexerRepository.AddToCache(item.RoomName);
            
        };

        return rooms;
    }

    public override ChatDb Insert(ChatDb instance)
    {
        if (Models.Add(instance) == true)
        {
            _roomIndexerRepository.AddToCache(instance.RoomName);
            return instance;
        }

        throw new Exception("Key already present.");

    }

    protected override ChatDb CoreGet(Predicate<ChatDb> selector)
    {
        var result =  base.CoreGet(selector);
    
        if(result is not null)
        {
            _roomIndexerRepository.AddToCache(result.RoomName);
        }

        return result;
    }


    public async override Task<bool> RemoveAsync(Predicate<ChatDb> selector)
    {
        var result = false;
        foreach (var model in Models)
        {
            if (selector(model))
            {
                result = Models.Remove(model);
                if(result == true)
                {
                    _roomIndexerRepository.Remove(model.RoomName);
                }
            }
        }

        return result;
    }

    public async Task<bool> UpdateImagesAsync(Predicate<ChatDb> selector, IEnumerable<string> images)
    {
        foreach (var model in Models)
        {
            if (selector(model))
            {
                model.Files.AddRange(images);
                return true;
            }
        }

        return false;
    }
}

