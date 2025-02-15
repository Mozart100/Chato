using Arkovean.Chat.Services.Validations;
using Chato.Server.Infrastracture;
using FluentValidation.Results;
using Serilog.Core;

namespace Chato.Server.Services.Validations
{
    public abstract class ServiceValidatorBase
    {
        protected ServiceValidatorBase(ILogger logger)
        {
            Logger = logger;
        }


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
        public ILogger Logger { get; }

        protected void Validate(IEnumerable<ChatoError> errors)
        {
            if (errors.SafeAny())
            {
                var instance = new ChatoException(errors.ToArray());
                ThrowException(instance);
            }
        }


        protected void Validate(string propertyName, string reason)
        {
            var error = new ChatoError(propertyName, reason);
            var instance = new ChatoException(error);
            ThrowException(instance);
        }

        private void ThrowException(ChatoException chatoException)
        {
            Logger.LogError(chatoException, "ChatoException {Topic} {@Erros} {@Exception}", "Validation", chatoException.ChatoErrors, chatoException);

            throw chatoException;
        }

    }
}