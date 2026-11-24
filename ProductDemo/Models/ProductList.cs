using System;
using System.Collections.Generic;

namespace ProductDemo.Models;

public partial class ProductList
{
    public int IdProduct { get; set; }

    public string Article { get; set; } = null!;

    public int IdProductType { get; set; }

    public int IdUnit { get; set; }

    public decimal Price { get; set; }

    public int IdProvider { get; set; }

    public int IdProduction { get; set; }

    public int IdCategory { get; set; }

    public decimal? Discount { get; set; }

    public int Count { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual ProductType IdProductTypeNavigation { get; set; } = null!;

    public virtual Production IdProductionNavigation { get; set; } = null!;

    public virtual Provider IdProviderNavigation { get; set; } = null!;

    public virtual Unit IdUnitNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
