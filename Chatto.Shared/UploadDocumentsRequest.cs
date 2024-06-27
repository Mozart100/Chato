using Microsoft.AspNetCore.Http;

namespace Chatto.Shared;

public class UploadDocumentsRequest
{
    public IFormFile Document1 { get; set; }
    public IFormFile Document2 { get; set; }
    public IFormFile Document3 { get; set; }
    public IFormFile Document4 { get; set; }
    public IFormFile Document5 { get; set; }
}

public class UploadDocumentsResponse
{
    public bool Document1 { get; set; } = false;
    public bool Document2 { get; set; } = false;
    public bool Document3 { get; set; } = false;
    public bool Document4 { get; set; } = false;
    public bool Document5 { get; set; } = false;
}
