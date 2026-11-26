using ProductDemo.Service;
using System.Windows;

namespace ProductDemo.ViewModel
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string? Status { get; set; }
        public DateOnly DataOrder { get; set; }
        public DateOnly DataDelivery { get; set; }
        public string? Pvz { get; set; }
        public string? Client { get; set; }
        public string? Composition { get; set; }
        public decimal Total { get; set; }
        public bool CanDelete { get; set; }

        public string DataOrderText => DataOrder.ToString("dd.MM.yyyy");

        public string DataDeliveryText => DataDelivery.ToString("dd.MM.yyyy");
        public Visibility DeleteVisibility =>
            CanDelete ? Visibility.Visible : Visibility.Collapsed;
    }
    public class OrderItemViewModel
    {
        private DiscountService _priceService = new DiscountService();
        public int IdProduct { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int Count { get; set; }

        public decimal FinalPrice => _priceService.GetFinalPrice(Price, Discount);

        public decimal Sum => FinalPrice * Count;

        public decimal FullSum => Price * Count;
    }

    public class ListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}