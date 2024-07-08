namespace Chatto.Shared
{
    public class ResponseWrapper<TResponse>  where TResponse : class
    {
        public bool IsSucceeded { get; set; }
        public TResponse Body { get; set; }
        public int StatusCode { get; set; }
    }


    
}

