using AutoMapper;
using Chato.Server.Configuration;
using Chato.Server.DataAccess.Models;
using Chatto.Shared;

namespace Chato.Server.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<UserDb,User>();
        CreateMap< User,UserDto>();



        CreateMap<CacheEvictionRoomConfig, CacheEvictionRoomConfigDto>();
    }
}