namespace Chato.Server.DataAccess.Models;

public class UserDb : EntityDbBase
{
    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }

    public string UserName { get; private set; }
    
    //public string Password { get; init; }
    public byte [] PasswordHash { get; init; }
    
    public string ConnectionId { get; set; }


    public List<string> Rooms = new List<string>();
}

