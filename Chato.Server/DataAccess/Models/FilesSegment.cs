using Chato.Server.Infrastracture;

namespace Chato.Server.DataAccess.Models;

public class FilesSegment
{
    public const int Max_Files = 5;

    private int _current = 0;
    private string[] _files = new string[Max_Files];

    public IEnumerable<string> GetImages() => _files.Take(_current).Where(x => x.IsNotEmpty() == false).ToArray();

    public void Add(string filePath)
    {
        _files[_current] = filePath;
        _current = _current % Max_Files;
    }

    internal void AddRange(IEnumerable<string> imageUrls)
    {
        foreach (var imageUrl in imageUrls)
        {
            Add(imageUrl);
        }
    }
}