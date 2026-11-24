using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Pvz
{
    public int IdPvz { get; set; }

    public int Index { get; set; }

    public int IdCity { get; set; }

    public string Street { get; set; } = null!;

    public string? HumberHouse { get; set; }

    public virtual City IdCityNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
