// Purpose:
// Wraps a list of games from the API response

namespace MeepleSystemClient.Models
{
    public class GetGamesResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public List<Game>? gameList { get; set; }
    }
}