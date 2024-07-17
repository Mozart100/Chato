using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Chato.Server.Controllers
{
    //[FeatureGate()]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        public ConfigurationController()
        {
            
        }

        [HttpGet]
        public async Task<bool> GetConfiguration()
        {

            return true;
        }
    }
}
