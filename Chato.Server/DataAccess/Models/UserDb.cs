using Chatto.Shared;

namespace Chato.Server.DataAccess.Models;

public class User : EntityDbBase, IUserEnittyMapper
{
    public string UserName { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
    public string[] Chats { get; set; }
    public string ConnectionId { get; set; }

}


public record UserFileInfo(string FileName, byte[] Content);

public class UserDb : User
{

    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }

    //public byte[] PasswordHash { get; init; }

    public List<UserFileInfo> Files { get; set; } = new List<UserFileInfo>();

    //public UserFileInfo Document1 { get; set; }
    //public UserFileInfo Document2 { get; set; }
    //public UserFileInfo Document3 { get; set; }
    //public UserFileInfo Document4 { get; set; }
    //public UserFileInfo Document5 { get; set; }
}