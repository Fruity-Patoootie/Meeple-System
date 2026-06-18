using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Style
{
    public int StyleId { get; set; }

    public string? StyleName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
