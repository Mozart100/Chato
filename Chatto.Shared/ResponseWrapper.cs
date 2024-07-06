namespace Chatto.Shared
{
    public class ResponseWrapper
    {
        public bool IsSucceeded { get; set; }
        public object Response { get; set; }
        public int StatusCode { get; set; }
    }

}
