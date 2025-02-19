using Chato.Server.Services;

namespace Chato.Server.Infrastracture;


public enum FileType
{
    Image,
    Text
}

public static class FileHelper
{
    public static FileType GetFileType(byte[] bytes)
    {
        if (bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8)
        {
            return FileType.Image;
        }
        else
        {
            return FileType.Text;
        }
    }

    public static bool IsFileText(byte[] ptr)
    {
        return GetFileType(ptr) == FileType.Text;
    }

    public static async Task<IEnumerable<(string FileName, byte[] Content)>> DissectAsync(IEnumerable<IFormFile> documents)
    {
        var list = new List<(string FileName, byte[] Content)>();

        foreach (var document in documents.SafeToArray())
        {
            using (var memoryStream = new MemoryStream())
            {
                await document.CopyToAsync(memoryStream);
                var documentBytes = memoryStream.ToArray();
                list.Add((document.FileName, documentBytes));
            }
        }

        return list;
    }

    public static void SaveFile(byte[] content ,string fileFullPath)
    {
        var wwwRootPath = Path.GetDirectoryName(fileFullPath);
        if (Directory.Exists(wwwRootPath) == false)
        {
            Directory.CreateDirectory(wwwRootPath);
        }

        File.WriteAllBytes(fileFullPath, content);
    }
}

