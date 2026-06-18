namespace MeepleSystemAPI.Models
{
    public class BarcodeCheckResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public GameData? Data { get; set; }
    }
}
