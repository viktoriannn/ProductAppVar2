using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class OrderItem
{
    public int IdOrderItem { get; set; }

    public int IdProduct { get; set; }

    public int Count { get; set; }

    public int IdOrder { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual ProductList IdProductNavigation { get; set; } = null!;
}
