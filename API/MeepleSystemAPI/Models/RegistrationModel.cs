
using System.ComponentModel.DataAnnotations;
namespace MeepleSystemAPI.Models
{
    public class RegistrationModel
    {
        [Required]
        public required string Title { get; set; }
        public string? Barcode { get; set; } = null;
        public string? StyleName { get; set; } = null;
        public decimal? Weight { get; set; } = null;
        public string? LocationName { get; set; } = null;
        public string? SupplierName { get; set; } = null;
        public string? SellerName { get; set; } = null;
        public string? ImageLocation { get; set; } = null;
        public decimal? Cost { get; set; } = null;
        public bool? NeedsAddedToBgg { get; set; } = null;
        public DateOnly? DateAcquired { get; set; } = null;
        public int? BggGameId { get; set; } = null;
        public bool? Expansion { get; set; } = null;
        public int? MinPlayers { get; set; } = null;
        public int? MaxPlayers { get; set; } = null;
        public string? RecommendedPlayers { get; set; } = null;
        public string? BestPlayers { get; set; } = null;
        public short? MinDuration { get; set; } = null;
        public short? MaxDuration { get; set; } = null;
    }
}
