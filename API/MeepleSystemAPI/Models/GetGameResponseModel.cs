using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class GetGameResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public GameData Game { get; set; }
    }
}
