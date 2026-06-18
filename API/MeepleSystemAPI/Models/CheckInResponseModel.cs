using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class CheckInResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public CheckInData? Data { get; set; }
    }
}