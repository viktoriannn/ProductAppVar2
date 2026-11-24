namespace ProductDemo.Service
{
    public class DiscountService
    {
        public decimal GetDiscount(decimal? discount)
        {
            return discount ?? 0;
        }

        public bool HasDiscount(decimal? discount)
        {
            return GetDiscount(discount) > 0;
        }

        // Скидка больше 15% - строка подсвечивается #2E8B57
        public bool IsHighDiscount(decimal? discount)
        {
            return GetDiscount(discount) > 15;
        }

        public decimal GetFinalPrice(decimal price, decimal? discount)
        {
            return price * (100 - GetDiscount(discount)) / 100;
        }

        public string GetDiscountText(decimal? discount)
        {
            return $"{GetDiscount(discount)}%";
        }
    }
}