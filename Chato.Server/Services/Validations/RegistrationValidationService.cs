using Arkovean.Chat.Services.Validations;
using Chato.Server.Models.Dtos;
using FluentValidation;

namespace Chato.Server.Services.Validations
{
    public interface IRegistrationValidationService
    {
        Task RegistrationRequestValidateAsync(RegistrationRequest request);
    }


    public class RegistrationValidationService : ServiceValidatorBase, IRegistrationValidationService
    {
        public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
        {
            public RegistrationRequestValidator()
            {
                RuleFor(req => req.UserName)
                 .NotEmpty()
                 .WithMessage("{PropertyName} shouldn't be empty.");

                RuleFor(req => req.Age)
                    .GreaterThanOrEqualTo(18)
                    .WithMessage("{PropertyName} should be at least 18.");

                RuleFor(req => req.Gender)
                    .NotEmpty()
                    .WithMessage("{PropertyName} shouldn't be empty.");

            }
        }


        private readonly ILogger<RegistrationValidationService> _logger;
        private readonly IUserService _userService;

        public RegistrationValidationService(ILogger<RegistrationValidationService> logger, IUserService userService)
        {
            _logger = logger;
            this._userService = userService;
        }

        public async Task RegistrationRequestValidateAsync(RegistrationRequest request)
        {
            var validator = new RegistrationRequestValidator();
            var validationResult = validator.Validate(request);

            var errors = Dissect(validationResult);

            Validate(errors);

            var result = await _userService.GetUserByNameOrIdGetOrDefaultAsync(request.UserName);
            if(result is not null)
            {
                var error = new ChatoError(nameof(request.UserName), "This userName is already taken - Please choose different.");
                var instance = new ChatoException(error);
                throw instance;

            }
        }
    }
}