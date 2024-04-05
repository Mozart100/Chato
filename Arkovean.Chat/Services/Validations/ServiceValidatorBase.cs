using Arkovean.Chat.Infrastracture;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Arkovean.Chat.Services.Validations
{
    public abstract class ServiceValidatorBase
    {
        protected virtual IList<SailPointError> Dissect(ValidationResult validationResult)
        {
            var errors = new List<SailPointError>();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(new SailPointError(errorMessage: error.ErrorMessage, propertyName: error.PropertyName));
                }
            }

            return errors;
        }

        protected void Validate(IEnumerable<SailPointError> errors)
        {
            if (errors.SafeAny())
            {
                var instance = new SailPointException(errors.ToArray());
                throw instance;
            }
        }
    }
}