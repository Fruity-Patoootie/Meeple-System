using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class GameCategory
{
    public int GameCategoryId { get; set; }

    public int? CategoryId { get; set; }

    public int? GameId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Game? Game { get; set; }
}
