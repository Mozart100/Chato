using Arkovean.Chat.Services.Validations;
using Chato.Server.Infrastracture;

namespace Chato.Server.Services.Validations
{
    public interface IUserValidationService
    {
        Task UploadDocumentsValidationAsync(IEnumerable<IFormFile> documents);
    }

    public class UserValidationService : ServiceValidatorBase, IUserValidationService
    {
        private const string Document_Name = "Documents";
        private const int Amount_Allowed_Document = 5;

        public UserValidationService(ILogger<UserValidationService> logger) : base(logger)
        {
        }

        public async Task UploadDocumentsValidationAsync(IEnumerable<IFormFile> documents)
        {
            if (documents.IsNullOrEmpty())
            {
                Validate([new ChatoError(Document_Name, "Cannot be empty")]);
            }

            if (documents.Count() > Amount_Allowed_Document)
            {
                Validate([new ChatoError(Document_Name, $"You are allowed to upload no more than {Amount_Allowed_Document}")]);
            }
        }
    }
}