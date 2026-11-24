using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Production
{
    public int IdProduction { get; set; }

    public string Production1 { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
