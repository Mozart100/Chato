namespace Arkovean.Chat.Models.Dtos
{
    public class AddCityRequest
    {
        public string City { get; set; }
    }


    public class AddCityResponse
    {
        public int Id { get; set; }
        public string City { get; set; }
    }
}