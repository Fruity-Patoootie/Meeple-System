namespace MeepleSystemClient.Models
{
    public class GameData
    {
        public int GameId { get; set; }
        public required string Title { get; set; }
        public required string? Barcode { get; set; }
        public required string? StyleName { get; set; }
        public required decimal? Weight { get; set; }
        public required int? NumberOfTimesPlayed { get; set; }
        public string? LocationName { get; set; }
        public string? SupplierName { get; set; }
        public string? SellerName { get; set; }
        public string? ImageLocation { get; set; }
        public decimal? Cost { get; set; }
        public bool? NeedsAddedToBgg { get; set; }
        public DateOnly? DateAcquired { get; set; }
        public int? BggGameId { get; set; }
        public bool? Expansion { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public string? RecommendedPlayers { get; set; }
        public string? BestPlayers { get; set; }
        public short? MinDuration { get; set; }
        public short? MaxDuration { get; set; }
    }
}
