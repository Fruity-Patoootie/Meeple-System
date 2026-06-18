using MeepleSystemAPI.Data;

namespace MeepleSystemAPI.Models
{
    public class GameCsvModel
    {
        public string Title { get; set; }

        public string? Barcode { get; set; }

        public string? LocationName { get; set; }

        public string? SupplierName { get; set; }

        public string? SellerName { get; set; }

        public string? ImageLocation { get; set; }

        public string? Cost { get; set; }

        public string? Weight { get; set; }

        public string? NeedsAddedToBgg { get; set; }

        public string? DateAcquired { get; set; }

        public string? StyleName { get; set; }

        public string? BggGameId { get; set; }

        public string? Expansion { get; set; }

        public string? MinPlayers { get; set; }

        public string? MaxPlayers { get; set; }

        public string? RecommendedPlayers { get; set; }

        public string? BestPlayers { get; set; }

        public string? MinDuration { get; set; }

        public string? MaxDuration { get; set; }
    }
}