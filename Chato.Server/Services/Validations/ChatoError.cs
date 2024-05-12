namespace Arkovean.Chat.Services.Validations
{
    public class ChatoError
    {
        public ChatoError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public string PropertyName { get; }

    }


    public class ChatoException : Exception
    {
        public ChatoException(params ChatoError[] sailPointErrors)
        {
            ChatoErrors = sailPointErrors;
        }

        public ChatoError[] ChatoErrors { get; }
    }
}