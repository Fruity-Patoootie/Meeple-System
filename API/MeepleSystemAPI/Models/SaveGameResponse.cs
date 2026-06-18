using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class SaveGameResponse
    {
        public string? GameTitle { get; set; }
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string? Message { get; set; } = null;
        public string? Token { get; set; } = null;
        public Game? Data { get; set; } = null;
        public List<Game>? DataList { get; set; } = null;
        public Error? Errors { get; set; } = null;
        public int GameId { get; set; }

    }

    public class Error
    {
        public string? Code { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}