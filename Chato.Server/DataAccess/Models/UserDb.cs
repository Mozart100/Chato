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

//public struct UserDto
//{
//    public string UserName { get; init; }

//    public byte[] PasswordHash { get; init; }

//    public string ConnectionId { get; set; }

//    public string[] Rooms { get; init; }

   
//    public UserDto(string userName, byte[] passwordHash, string connectionId, string[] rooms)
//    {
//        UserName = userName;
//        PasswordHash = passwordHash;
//        ConnectionId = connectionId;
//        Rooms = rooms ;
//    }

//    public override int GetHashCode() => UserName.GetHashCode();

//    public override bool Equals(object? obj) => obj is UserDb user && UserName.Equals(user.Id);
//}
