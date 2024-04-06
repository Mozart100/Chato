//namespace Arkovean.Chat.Services
//{
//    public interface ICityLocaterService
//    {
//        Task<IEnumerable<string>> GetAllCitiesAsync();
//        Task<IEnumerable<string>> SearchCitiesAsync(string prefix, int level = 2);
//        Task StoreCityAsync(string city);
//    }

//    public class CityLocaterService : ICityLocaterService
//    {
//        private readonly GraphCities _graphCity;

//        public CityLocaterService()
//        {
//            _graphCity = new GraphCities();
//        }

//        public async Task StoreCityAsync(string city)
//        {
//            _graphCity.InsertCity(city);
//        }

//        public async Task<IEnumerable<string>> SearchCitiesAsync(string prefix, int level = 2)a
//        {
//            var cities = _graphCity.SearchCities(prefix, level);
//            return cities;
//        }

//        public async Task<IEnumerable<string>> GetAllCitiesAsync()
//        {
//            var cities = await _graphCity.GetAllCitiesAsync();
//            return cities;
//        }
//    }
//}