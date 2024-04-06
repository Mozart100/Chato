using Arkovean.Chat.Models.Dtos;

namespace Arkovean.Chat.Services.Validations
{
    public interface ICityLocaterValidationService
    {

    }
    public class CityLocaterValidationService : ServiceValidatorBase, ICityLocaterValidationService
    {
        private readonly ILogger<CityLocaterValidationService> _logger;

        public CityLocaterValidationService(ILogger<CityLocaterValidationService> logger)
        {
            _logger = logger;
        }

        public async Task AddCarRequestValidateAsync(GetCitiesRequest request)
        {
            var validator = new GetCitiesRequestValidator();
            var validationResult = validator.Validate(request);

            var errors = Dissect(validationResult);

            Validate(errors);
        }
    }
}