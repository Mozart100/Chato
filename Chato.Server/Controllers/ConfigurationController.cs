using AutoMapper;
using Chato.Server.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;

namespace Chato.Server.Controllers
{
    //[FeatureGate()]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        public const string EvictionUrl = "Eviction";
        private readonly IMapper _mapper;
        private CacheEvictionRoomConfig _config;

        public ConfigurationController( IMapper mapper, IOptions<CacheEvictionRoomConfig> config)
        {
            _config = config.Value;
            this._mapper = mapper;
        }

        [Route(EvictionUrl)]
        [HttpGet]
        public async Task<CacheEvictionRoomConfigDto> GetEvictionConfiguration()
        {
            var dto = _mapper.Map<CacheEvictionRoomConfigDto>(_config);
            return dto;
        }
    }
}
