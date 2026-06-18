using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class GetGamesResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; } = null;
        public List<GameData>? gameList { get; set; } = null;
    }
}
