using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Infrastructure.Persistence;
namespace Shop.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly ShopDbContext _context;

    public PromotionRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<KhuyenMai>> GetActivePromotionsAsync(int productId, DateTime now)
    {
        return await _context.SanPhamKhuyenMais
            .Include(spkm => spkm.KhuyenMai)
            .Where(spkm =>
                spkm.MaSanPham == productId &&
                spkm.KhuyenMai != null &&
                spkm.KhuyenMai.KichHoat &&
                spkm.KhuyenMai.NgayBatDau <= now &&
                spkm.KhuyenMai.NgayKetThuc >= now
            )
            .Select(spkm => spkm.KhuyenMai!)
            .ToListAsync();
    }
}