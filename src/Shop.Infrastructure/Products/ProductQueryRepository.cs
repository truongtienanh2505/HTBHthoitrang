using Microsoft.EntityFrameworkCore;
using Shop.Application.Products;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Products;

public class ProductQueryRepository : IProductQueryRepository
{
    private readonly ShopDbContext _db;
    public ProductQueryRepository(ShopDbContext db) => _db = db;

    public async Task<List<ProductCardDto>> GetProductCardsAsync(int skip, int take, CancellationToken ct)
    {
        skip = Math.Max(0, skip);
        take = Math.Clamp(take, 1, 100);

        var sql = """
        SELECT
            sp.MaSanPham,
            sp.TenSanPham,
            sp.Slug,
            COALESCE(img.UrlAnh, sp.AnhDaiDien) AS Anh,
            MIN(pp.GiaGoc) AS GiaThapNhat,
            MIN(pp.GiaSauGiam) AS GiaSauGiamThapNhat
        FROM dbo.SanPham sp
        OUTER APPLY (
            SELECT TOP (1) a.UrlAnh
            FROM dbo.AnhSanPham a
            WHERE a.MaSanPham = sp.MaSanPham
              AND a.AnhChinh = 1
            ORDER BY a.ThuTu ASC, a.MaAnh ASC
        ) img
        JOIN dbo.ProductPromotions pp ON pp.MaSanPham = sp.MaSanPham
        WHERE sp.HoatDong = 1
        GROUP BY sp.MaSanPham, sp.TenSanPham, sp.Slug, COALESCE(img.UrlAnh, sp.AnhDaiDien)
        ORDER BY sp.MaSanPham DESC
        OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY
        """;

        var rows = await _db.ProductCardRows
            .FromSqlRaw(sql, skip, take)
            .AsNoTracking()
            .ToListAsync(ct);

        return rows.Select(r => new ProductCardDto(
            r.MaSanPham, r.TenSanPham, r.Slug, r.Anh, r.GiaThapNhat, r.GiaSauGiamThapNhat
        )).ToList();
    }
}