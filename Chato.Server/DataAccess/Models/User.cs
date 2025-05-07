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
    public string? ConnectionId { get; set; } = null;


    public IEnumerable<string> Files => FileSegment.GetImages();

    //--------------------------------------------------------------------------------------
    public FilesSegment FileSegment { get; set; } = new FilesSegment();
}