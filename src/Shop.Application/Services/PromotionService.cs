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
                var best = promotions
                    .OrderByDescending(p =>
                        p.LoaiGiamGia == "PERCENTAGE"
                            ? originalPrice * (p.GiaTriGiam / 100)
                            : p.GiaTriGiam)
                    .First();

                if (best.LoaiGiamGia == "PERCENTAGE")
                    finalPrice -= originalPrice * (best.GiaTriGiam / 100);
                else
                    finalPrice -= best.GiaTriGiam;
            }

            return Math.Max(finalPrice, 0);
        }
    }
}