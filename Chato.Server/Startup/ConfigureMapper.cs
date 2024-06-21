﻿using AutoMapper;
using Chato.Server.DataAccess.Models;

namespace Chato.Server.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<UserDb,User>();
        //CreateMap<CityDb, AddCityResponse>();
    }
}