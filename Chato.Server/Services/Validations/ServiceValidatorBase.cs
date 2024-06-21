using Arkovean.Chat.Services.Validations;
using Chato.Server.Infrastracture;
using FluentValidation.Results;

namespace Chato.Server.Services.Validations
{
    public abstract class ServiceValidatorBase
    {
        protected virtual IList<ChatoError> Dissect(ValidationResult validationResult)
        {
            var errors = new List<ChatoError>();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(new ChatoError(errorMessage: error.ErrorMessage, propertyName: error.PropertyName));
                }
            }

            return errors;
        }

        protected void Validate(IEnumerable<ChatoError> errors)
        {
            if (errors.SafeAny())
            {
                var instance = new ChatoException(errors.ToArray());
                throw instance;
            }
        }
    }
}