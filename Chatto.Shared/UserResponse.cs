namespace Chatto.Shared;

public enum ChatType
{
    Public, Private
}
public record ParticipantInChat(string ChatName, ChatType ChatType);

public interface IAutomapperEntities
{

}

public interface IUserEnittyMapper : IAutomapperEntities
{
    string UserName { get; set; }
    int Age { get; set; }
    string Description { get; set; }
    string Gender { get; set; }
    ParticipantInChat [] Chats { get; set; }
    string ConnectionId { get; set; }
}


public class UserDto : IUserEnittyMapper
{
    public string UserName { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
    public ParticipantInChat[] Chats { get; set; }
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
