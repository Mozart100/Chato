using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.DataAccess.Models;

public class User : EntityDbBase, IUserEnittyMapper
{
    public const string User_Key = $"{nameof(User_Key)}";

    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }



    public string UserName { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
    public ParticipantInChat[] Chats { get; set; }
    public string ConnectionId { get; set; }

    public FilesSegment FileSegment { get; set; } = new FilesSegment();
}

public class FilesSegment
{
    public const int Max_Files = 5;
    private int _current = 0;
    private string[] _files = new string[Max_Files];


    public void Add(string filePath)
    {
        _files[_current] = filePath;
        _current = _current % Max_Files;
    }

    public IEnumerable<string> GetImages() => _files.Take(_current).Where(x => x.IsNotEmpty() == false).ToArray();

}