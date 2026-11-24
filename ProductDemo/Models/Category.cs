using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string Category1 { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
