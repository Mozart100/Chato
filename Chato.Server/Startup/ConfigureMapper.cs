using AutoMapper;
using Chato.Server.Configuration;
using Chato.Server.DataAccess.Models;
using Chatto.Shared;

namespace Chato.Server.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap< User,UserDto>();
        CreateMap<Chat, ChatDto>();
             //.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.UserMessages));



        CreateMap<CacheEvictionRoomConfig, CacheEvictionRoomConfigDto>();
    }
}