using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class EditGameResponse
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string? Message { get; set; } = null;
        public GameData? Data { get; set; } = null;
    }
}
