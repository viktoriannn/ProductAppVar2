using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Unit
{
    public int IdUnit { get; set; }

    public string Unit1 { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
