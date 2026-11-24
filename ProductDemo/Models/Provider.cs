using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Provider
{
    public int IdProvider { get; set; }

    public string Provider1 { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
