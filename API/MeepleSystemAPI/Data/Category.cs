using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<GameCategory> GameCategories { get; set; } = new List<GameCategory>();
}
