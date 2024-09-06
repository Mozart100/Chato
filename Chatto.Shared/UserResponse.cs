namespace Chatto.Shared;

public interface IAutomapperEntities
{

}

public interface IUserEnittyMapper : IAutomapperEntities
{
    string UserName { get; set; }
    int Age { get; set; }
    string Description { get; set; }
    string Gender { get; set; }
    string[] Rooms { get; set; }
    string ConnectionId { get; set; }
}


public class UserDto : IUserEnittyMapper
{
    public string UserName { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
    public string[] Rooms { get; set; }
    public string ConnectionId { get; set; }

}
public class UserResponse
{

    public UserDto User { get; set; }
    //public string Username { get; set; } = string.Empty;
    ////public byte[] PasswordHash { get; set; }
    ////public byte[] PasswordSalt { get; set; }
    //public string RefreshToken { get; set; } = string.Empty;
    //public DateTime TokenCreated { get; set; }
    //public DateTime TokenExpires { get; set; }
}
