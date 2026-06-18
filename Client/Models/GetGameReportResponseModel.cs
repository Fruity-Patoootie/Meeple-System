// Purpose:
// Wraps a list of games from the API response

using System.Text.Json.Serialization;

namespace MeepleSystemClient.Models
{
    public class GetGameReportResponseModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        [JsonPropertyName("gameList")]
        public List<ReportGame>? GameList { get; set; }
    }
}