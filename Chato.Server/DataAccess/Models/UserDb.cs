namespace Chato.Server.DataAccess.Models;

public class UserDb : EntityDbBase
{
    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }

    public string UserName { get; set; }
    public string ConnectionId { get; set; }
}

