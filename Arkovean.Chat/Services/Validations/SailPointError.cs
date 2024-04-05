namespace Arkovean.Chat.Services.Validations
{
    public class SailPointError
    {
        public SailPointError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public string PropertyName { get; }

    }


    public class SailPointException : Exception
    {
        public SailPointException(params SailPointError[] sailPointErrors)
        {
            SailPointErrors = sailPointErrors;
        }

        public SailPointError[] SailPointErrors { get; }
    }
}