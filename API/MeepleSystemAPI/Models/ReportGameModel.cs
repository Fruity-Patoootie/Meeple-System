using System.Runtime.InteropServices.Marshalling;

namespace MeepleSystemAPI.Models
{
    public class ReportGameModel
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public int TimesPlayed { get; set; }
        public string? Location { get; set; }
    }
}
