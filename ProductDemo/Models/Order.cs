using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public DateOnly DataOrder { get; set; }

    public DateOnly DataDelivery { get; set; }

    public int IdPvz { get; set; }

    public int IdUser { get; set; }

    public int Code { get; set; }

    public string Status { get; set; } = null!;

    public virtual Pvz IdPvzNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
