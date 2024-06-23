namespace Chato.Server.DataAccess.Models;

public class User : EntityDbBase 
{
    public string UserName { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
    public string[] Rooms { get; set; }
    public string ConnectionId { get; set; }

}

public class UserDb : User
{
    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }

    //public byte[] PasswordHash { get; init; }

    public byte [] Document1 { get; set; }
    public byte [] Document2 { get; set; }
    public byte [] Document3 { get; set; }
    public byte [] Document4 { get; set; }
    public byte [] Document5 { get; set; }
}