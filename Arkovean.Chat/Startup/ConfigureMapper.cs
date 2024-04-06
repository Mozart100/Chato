using Arkovean.Chat.DataAccess.Models;
using Arkovean.Chat.Models.Dtos;
using AutoMapper;

namespace NetBet.Startup
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
            CreateMap<AddCityRequest, CityDb>();
            CreateMap<CityDb, AddCityResponse>();
        }
    }
}