namespace Chato.Automation.Responses
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public string Level { get; set; }
    }



    public class ErrorResponseException : Exception
    {
        public ErrorResponse ErrorResponse { get; set; }
    }
}