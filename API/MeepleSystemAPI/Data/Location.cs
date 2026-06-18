using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Location
{
    public int LocationId { get; set; }

    public string? LocationName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
