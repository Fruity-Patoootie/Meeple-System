using System;
using System.Collections.Generic;

namespace MeepleSystemAPI.Data;

public partial class Seller
{
    public int SellerId { get; set; }

    public string? SellerName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
