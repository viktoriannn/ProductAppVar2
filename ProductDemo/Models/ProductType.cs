using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class ProductType
{
    public int IdProductType { get; set; }

    public string ProductType1 { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
