using Arkovean.Chat.Services.Validations;
using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;

namespace Chato.Server.Services.Validations
{
    public interface IChatValidationService
    {
        Task UploadDocumentsValidationAsync(User user, string chatName , IEnumerable<IFormFile> documents);
    }

    public class ChatValidationService : ServiceValidatorBase, IChatValidationService
    {
        private const string Document_Name = "Documents";
        private const int Amount_Allowed_Document = 5;

        public ChatValidationService(ILogger<UserValidationService> logger) : base(logger)
        {
        }


        public async Task UploadDocumentsValidationAsync(User user, string chatName ,IEnumerable<IFormFile> documents)
        {
            if (documents.IsNullOrEmpty())
            {
                Validate([new ChatoError(Document_Name, "Cannot be empty")]);
            }

            if (documents.Count() > Amount_Allowed_Document)
            {
                Validate([new ChatoError(Document_Name, $"You are allowed to upload no more than {Amount_Allowed_Document}")]);
            }

            var participatedChat = user.Chats.FirstOrDefault(x => x.ChatName == chatName);
            if (participatedChat is null)
            {
                Validate([new ChatoError(chatName, $"You are not allowed to upload files to this chat.")]);
            }
        }
    }
}