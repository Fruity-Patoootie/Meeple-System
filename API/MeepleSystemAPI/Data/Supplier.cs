using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
