using Microsoft.EntityFrameworkCore;

public class PromotionService : IPromotionService
{
    private readonly ShopDbContext _context;

    public PromotionService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> TinhGiaSauGiamAsync(decimal giaGoc, int promotionId)
    {
        var promotion = await _context.Promotions
            .FirstOrDefaultAsync(x => x.Id == promotionId);

        if (promotion == null)
            return giaGoc;

        if (!promotion.IsActive)
            return giaGoc;

        if (DateTime.Now < promotion.NgayBatDau || DateTime.Now > promotion.NgayKetThuc)
            return giaGoc;

        decimal giaSauGiam = giaGoc;

        if (promotion.LoaiGiam == 1) // %
        {
            giaSauGiam = giaGoc - (giaGoc * promotion.GiaTriGiam / 100);
        }
        else if (promotion.LoaiGiam == 2) // tiền
        {
            giaSauGiam = giaGoc - promotion.GiaTriGiam;
        }

        if (giaSauGiam < 0)
            giaSauGiam = 0;

        return giaSauGiam;
    }
    public async Task<decimal> CalculateFinalPriceAsync(int productId, decimal originalPrice)
{   
    var now = DateTime.Now;

    var promotions = await _context.Promotions
        .Where(p => p.IsActive 
            && p.NgayBatDau <= now 
            && p.NgayKetThuc >= now)
        .ToListAsync();

    if (!promotions.Any())
        return originalPrice;

    // ❌ Không cho trùng loại
    var hasDuplicateType = promotions
        .GroupBy(p => p.LoaiGiamGia)
        .Any(g => g.Count() > 1);

    if (hasDuplicateType)
        throw new Exception("Không được áp dụng nhiều khuyến mãi cùng loại");

    decimal finalPrice = originalPrice;

    // 1️⃣ Áp dụng giảm %
    var percentPromo = promotions
        .FirstOrDefault(p => p.LoaiGiamGia == PromotionType.Percentage);

    if (percentPromo != null)
    {
        finalPrice -= finalPrice * (percentPromo.GiaTri / 100);
    }

    // 2️⃣ Áp dụng giảm tiền
    var fixedPromo = promotions
        .FirstOrDefault(p => p.LoaiGiamGia == PromotionType.FixedAmount);

    if (fixedPromo != null)
    {
        finalPrice -= fixedPromo.GiaTri;
    }

    return finalPrice < 0 ? 0 : finalPrice;
}

}
