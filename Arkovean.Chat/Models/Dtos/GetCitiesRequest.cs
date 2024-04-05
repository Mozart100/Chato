namespace Arkovean.Chat.Models.Dtos
{
    public class GetCitiesRequest
    {
        public string Prefix { get; set; }
    }

    public class GetCitiesResponse
    {
        public string[] Cities { get; set; }
    }
}