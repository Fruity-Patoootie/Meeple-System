using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Game
{
    public int GameId { get; set; }

    public string Title { get; set; } = null!;

    public string? Barcode { get; set; }

    public int? LocationId { get; set; }

    public int? SupplierId { get; set; }

    public int? SellerId { get; set; }

    public string? ImageLocation { get; set; }

    public int? StyleId { get; set; }

    public decimal? Cost { get; set; }

    public decimal? Weight { get; set; }

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

    public virtual ICollection<GameCategory> GameCategories { get; set; } = new List<GameCategory>();

    public virtual Location? Location { get; set; }

    public virtual Seller? Seller { get; set; }

    public virtual Style? Style { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<TimePlayed> TimesPlayed { get; set; } = new List<TimePlayed>();
}
