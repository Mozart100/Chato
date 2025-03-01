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

    public ChatRepository(ILogger<ChatRepository> logger, IMapper mapper)
    : base(mapper)
    {
        _logger = logger;
    }

    public override IEnumerable<ChatDto> GetAll()
    {
        var rooms = base.GetAll();
        return rooms;
    }

    public override ChatDto Insert(Chat instance)
    {
        var dto = base.Insert(instance);
        return dto;
    }

    protected override Chat CoreGet(Predicate<Chat> selector)
    {
        var result = base.CoreGet(selector);
        return result;
    }


    public async override Task<bool> RemoveAsync(Predicate<Chat> selector)
    {
        foreach (var model in Models)
        {
            if (selector(model))
            {
                if (Models.Remove(model) == true)
                {
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

