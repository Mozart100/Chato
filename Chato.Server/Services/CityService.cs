//using Arkovean.Chat.DataAccess.Models;
//using Arkovean.Chat.DataAccess.Repository;
//using Arkovean.Chat.Models.Dtos;
//using Arkovean.Chat.Services.Validations;
//using AutoMapper;
//using Chato.Server.Infrastracture;

//namespace Arkovean.Chat.Services
//{
//    public interface ICityService
//    {
//        Task<CityDb> StoreCityAsync(AddCityRequest request);
//        Task<IEnumerable<CityDb>> StoreCityAsync(AddBatchCityRequest request);
//    }

//    public class CityService : ICityService
//    {
//        private readonly ICityRepository _cityRepository;
//        private readonly IMapper _mapper;
//        private readonly ICityValidationService _cityValidationService;
//        private readonly ICityLocaterService _cityLocaterService;

//        public CityService(ICityRepository cityRepository, IMapper mapper,
//            ICityValidationService cityValidationService,
//            ICityLocaterService cityLocaterService
//            )
//        {
//            this._cityRepository = cityRepository;
//            this._mapper = mapper;
//            this._cityValidationService = cityValidationService;
//            this._cityLocaterService = cityLocaterService;
//        }

//        public async Task<CityDb> StoreCityAsync(AddCityRequest request)
//        {
//            await _cityValidationService.AddCarRequestValidateAsync(request);
//            return await StoreCityAsync(request.City);
//        }

//        public async Task<IEnumerable<CityDb>> StoreCityAsync(AddBatchCityRequest request)
//        {
//            var citiesDb = new List<CityDb>();
//            if (request.Cities.SafeAny())
//            {
//                foreach (var city in request.Cities)
//                {
//                    if (!city.IsNullOrEmpty())
//                    {
//                        citiesDb.Add(await StoreCityAsync(city));
//                    }
//                }
//            }

//            return citiesDb;
//        }

//        public async Task<CityDb> StoreCityAsync(string city)
//        {
//            var cityDb = new CityDb() { City = city };
//            var cityDetailDb = await _cityRepository.InsertAsync(cityDb);
//            await _cityLocaterService.StoreCityAsync(cityDetailDb.City);

//            return cityDetailDb;
//        }
//    }
//}