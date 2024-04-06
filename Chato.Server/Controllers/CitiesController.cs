//using Arkovean.Chat.Models.Dtos;
//using Arkovean.Chat.Services;
//using Arkovean.Chat.Services.Validations;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;

//namespace Arkovean.Chat.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class CitiesController : ControllerBase
//    {
//        private readonly ICityService _cityDetailService;
//        private readonly ICityLocaterService _cityLocaterService;
//        private readonly IMapper _mapper;

//        public CitiesController(ICityService cityDetailService, ICityLocaterService cityLocaterService, IMapper mapper)
//        {
//            this._cityDetailService = cityDetailService;
//            this._cityLocaterService = cityLocaterService;
//            this._mapper = mapper;
//        }

//        [HttpPost]
//        public async Task<AddCityResponse> AddCity(AddCityRequest request)
//        {
//            var dbEntity = await _cityDetailService.StoreCityAsync(request);
//            var response = _mapper.Map<AddCityResponse>(dbEntity);
//            return response;
//        }

//        [HttpPost]
//        [Route("batch")]
//        public async Task<AddBatchCityResponse> AddBatchCities(AddBatchCityRequest request)
//        {
//            var dbEntities = await _cityDetailService.StoreCityAsync(request);
//            var buffer = _mapper.Map<AddCityResponse[]>(dbEntities);
//            var response = new AddBatchCityResponse { AddCityResponsees = buffer };
//            return response;
//        }


//        [HttpGet]
//        public async Task<GetCitiesResponse> GetAllCitities()
//        {
//            var cities = await _cityLocaterService.GetAllCitiesAsync();
//            return new GetCitiesResponse { Cities = cities.ToArray() };
//        }

//        [HttpGet]
//        [Route("{prefix}/level/{level}")]
//        public async Task<GetCitiesResponse> GetCitities(string prefix, int level)
//        {
//            if (level <= 0)
//            {
//                var error = new SailPointError("level", "Should be above zero.");
//                throw new SailPointException(new[] { error });
//            }

//            var cities = await _cityLocaterService.SearchCitiesAsync(prefix, level);
//            return new GetCitiesResponse { Cities = cities.ToArray() };
//        }
//    }
//}