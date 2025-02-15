using Microsoft.AspNetCore.Http;

namespace Chatto.Shared;

//public class UploadDocumentsRequest
//{
//    public IFormFile Document1 { get; set; }
//    public IFormFile Document2 { get; set; }
//    public IFormFile Document3 { get; set; }
//    public IFormFile Document4 { get; set; }
//    public IFormFile Document5 { get; set; }
//}


public abstract class UserImagesResponseBase
{
    public List<string> Files { get; set; } = new List<string>();

}
public class UploadDocumentsResponse : UserImagesResponseBase
{
}

public class GetAllUserImagesResponse : UserImagesResponseBase
{
}


