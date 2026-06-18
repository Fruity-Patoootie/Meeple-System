using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class TimePlayed
{
    public int TimePlayedId { get; set; }

    public int? GameId { get; set; }

    public DateOnly? Time { get; set; }

    public virtual Game? Game { get; set; }
}
