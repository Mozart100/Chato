using System.Text;
using Chato.Server.Infrastracture;

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

        //public override string ToString()
        //{
        //    var stringBuilder = new StringBuilder();
        //    stringBuilder.AppendLine("ChatoException Validation:");
        //    foreach (var item in ChatoErrors.SafeToArray())
        //    {
        //        stringBuilder.AppendLine($"{item.PropertyName} - {item.ErrorMessage}.");
        //    }
        //    var result = stringBuilder.ToString();
        //    return result;  
        //}
    }
}