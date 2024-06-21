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


}

