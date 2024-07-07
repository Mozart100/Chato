namespace Chatto.Shared
{
    public class ResponseWrapper<TResponse> where TResponse : class
    {
        public bool IsSucceeded { get; set; }
        public TResponse Response { get; set; }
        public int StatusCode { get; set; }
    }

}
