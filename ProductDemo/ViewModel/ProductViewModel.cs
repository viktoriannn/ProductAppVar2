using ProductDemo.Service;
using System.Windows;
using System.Windows.Media;

namespace ProductDemo.ViewModel
{
    public class ProductViewModel
    {
        private ImageService _imageService = new ImageService();
        private DiscountService _priceService = new DiscountService();
        public int Id { get; set; }
        public string? ProductType { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Production { get; set; }
        public string? Provider { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
        public int Count { get; set; }
        public decimal? Discount { get; set; }
        public string? Image { get; set; }
        public bool CanDelete { get; set; }
        public ImageSource Photo => _imageService.LoadImage(Image);
        public bool HasDiscount => _priceService.HasDiscount(Discount);

        public decimal FinalPrice => _priceService.GetFinalPrice(Price, Discount);

        public string DiscountText => _priceService.GetDiscountText(Discount);
        public Visibility DiscountVisibility =>
            HasDiscount ? Visibility.Visible : Visibility.Collapsed;

        public Visibility DeleteVisibility =>
            CanDelete ? Visibility.Visible : Visibility.Collapsed;
        public Brush Background
        {
            get
            {
                if (Count <= 0)
                    return Brushes.LightBlue;
                if (_priceService.IsHighDiscount(Discount))
                    return Brushes.SeaGreen;
                return Brushes.White;
            }
        }
    }
}