using Chato.Server.DataAccess.Models;

namespace Chato.Server.Services
{
    public class GeneratorDataService
    {


        public async Task<IEnumerable<ChatRoomDb>> GetFakeChatRoomData()
        {
            var rooms = new List<ChatRoomDb>();

            for (int i = 0; i<1; i++)
            {
                var room = new ChatRoomDb { Id = $"room_{i}" };

                var users = new List<string>();

                for(int j = 0; j < 3; j++)
                {
                    var userName = $"{room.Id}__User{j + 1}";
                    users.Add(userName);    
                }



            }

            return null;
        }
    }
}
