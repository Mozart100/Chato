using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Chatto.Shared;
using System;
using System.Reflection;
using System.Text;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRepository : IRepositoryBase<Chat, ChatDto>
{
    Task<bool> UpdateImagesAsync(Predicate<Chat> selector, IEnumerable<string> images);
}

public class ChatRepository : AutoRepositoryBase<Chat, ChatDto>, IChatRepository
{
    private readonly ILogger<ChatRepository> _logger;
    private readonly IRoomIndexerRepository _roomIndexerRepository;

    public ChatRepository(ILogger<ChatRepository> logger, IMapper mapper, IRoomIndexerRepository roomIndexerRepository)
    : base(mapper)
    {
        _logger = logger;
        this._roomIndexerRepository = roomIndexerRepository;
    }

    public override IEnumerable<ChatDto> GetAll()
    {
        var rooms = base.GetAll();
        foreach (var item in rooms)
        {
            _roomIndexerRepository.AddToCache(item.RoomName);

        };

        return rooms;
    }

    public override ChatDto Insert(Chat instance)
    {
        var dto = base.Insert(instance);
        if (dto is not null)
        {
            _roomIndexerRepository.AddToCache(instance.RoomName);
        }

        return dto;
    }

    protected override Chat CoreGet(Predicate<Chat> selector)
    {
        var result = base.CoreGet(selector);

        if (result is not null)
        {
            _roomIndexerRepository.AddToCache(result.RoomName);
        }

        return result;
    }


    public override bool Remove(Predicate<Chat> selector)
    {
        foreach (var model in Models)
        {
            if (selector(model))
            {
                if (Models.Remove(model) == true)
                {
                    _roomIndexerRepository.Remove(model.RoomName);
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<bool> UpdateImagesAsync(Predicate<Chat> selector, IEnumerable<string> images)
    {
        foreach (var model in Models)
        {
            if (selector(model))
            {
                model.FileSegment.AddRange(images);
                //model.Files.AddRange(images);
                return true;
            }
        }

        return false;
    }
}

