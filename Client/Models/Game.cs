using System.Text.Json.Serialization;

namespace MeepleSystemClient.Models
{
    public class Game
    {
        // ================= BASIC INFO =================

        public int GameId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        [JsonPropertyName("styleName")]
        public string? Style { get; set; }

        public float? Weight { get; set; }

        // ================= STATS =================

        [JsonPropertyName("numberOfTimesPlayed")]
        public int? NumberOfTimesPlayed { get; set; }

        // ================= LOCATION / SELLING =================

        [JsonPropertyName("locationName")]
        public string? LocationName { get; set; }

        public string? SupplierName { get; set; }

        public string? SellerName { get; set; }

        // ================= MEDIA =================

        public string? ImageLocation { get; set; }

        // ================= PURCHASE =================

        public decimal? Cost { get; set; }

        public bool? NeedsAddedToBgg { get; set; }

        public DateTime? DateAcquired { get; set; }

        public int? BggGameId { get; set; }

        // ================= GAMEPLAY =================

        public bool? Expansion { get; set; }

        public int? MinPlayers { get; set; }

        public int? MaxPlayers { get; set; }

        public string? RecommendedPlayers { get; set; }

        public string? BestPlayers { get; set; }

        public int? MinDuration { get; set; }

        public int? MaxDuration { get; set; }

        // ================= OPTIONAL FK IDs =================

        public int? LocationId { get; set; }

        public int? SupplierId { get; set; }

        public int? SellerId { get; set; }

        public int? StyleId { get; set; }
    }
}