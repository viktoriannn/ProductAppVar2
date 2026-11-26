namespace ProductDemo.Service
{
    public class DiscountService
    {
        public decimal GetDiscount(decimal? discount)
        {
            if (discount == null)
                return 0;
            else
                return discount.Value;
        }

        public bool HasDiscount(decimal? discount)
        {
            return GetDiscount(discount) > 0;
        }

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