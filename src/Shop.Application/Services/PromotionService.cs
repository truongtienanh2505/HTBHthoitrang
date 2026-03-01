using Shop.Application.Interfaces;

namespace Shop.Application.Services
{
    public class PromotionService
    {
        private readonly IPromotionRepository _repository;

        public PromotionService(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> CalculateDiscountedPrice(
            int productId,
            decimal originalPrice)
        {
            var now = DateTime.Now;

            var promotions = await _repository
                .GetActivePromotionsAsync(productId, now);

            decimal finalPrice = originalPrice;

           if (promotions != null && promotions.Any())
{
    // Tìm khuyến mãi tốt nhất dựa trên giá trị giảm thực tế
    var best = promotions
        .OrderByDescending(p =>
        {
            if (p.LoaiGiamGia == "PERCENTAGE")
            {
                var amount = originalPrice * (p.GiaTriGiam / 100);
                // Nếu có GiamToiDa, lấy giá trị nhỏ hơn giữa mức % và mức trần
                return p.GiamToiDa.HasValue ? Math.Min(amount, p.GiamToiDa.Value) : amount;
            }
            return p.GiaTriGiam;
        })
        .First();

    // Áp dụng giảm giá vào giá cuối cùng
    if (best.LoaiGiamGia == "PERCENTAGE")
    {
        var discountAmount = originalPrice * (best.GiaTriGiam / 100);
        // Kiểm tra lại GiamToiDa một lần nữa khi trừ tiền
        if (best.GiamToiDa.HasValue)
        {
            discountAmount = Math.Min(discountAmount, best.GiamToiDa.Value);
        }
        finalPrice -= discountAmount;
    }
    else
    {
        finalPrice -= best.GiaTriGiam;
    }
}

            return Math.Max(finalPrice, 0);
        }
    }
}