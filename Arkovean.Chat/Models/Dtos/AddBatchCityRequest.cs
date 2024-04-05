namespace Arkovean.Chat.Models.Dtos
{
    public class AddBatchCityRequest
    {
        public string[] Cities { get; set; }
    }


    public class AddBatchCityResponse
    {
        public AddCityResponse[] AddCityResponsees { get; set; }

    }
}