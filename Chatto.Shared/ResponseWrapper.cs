namespace Chatto.Shared
{
    public class ResponseWrapperSlim<TResponse> where TResponse : class
    {
        public bool IsSucceeded { get; set; }
        public TResponse Response { get; set; }
    }

        public class ResponseWrapper<TResponse> : ResponseWrapperSlim<TResponse> where TResponse : class
    {
        public int StatusCode { get; set; }
    }

}
