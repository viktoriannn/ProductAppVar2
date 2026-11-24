using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class City
{
    public int IdCity { get; set; }

    public string City1 { get; set; } = null!;

    public virtual ICollection<Pvz> Pvzs { get; set; } = new List<Pvz>();
}
