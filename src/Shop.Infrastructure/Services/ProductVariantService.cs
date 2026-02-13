using Microsoft.EntityFrameworkCore;
using Shop.Application.Products.DTOs;
using Shop.Application.Products.Interfaces;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly ShopDbContext _context;

    public ProductVariantService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task AddVariantAsync(CreateVariantDto dto)
    {
        var exists = await _context.ProductVariants
            .AnyAsync(x => x.SKU == dto.SKU);

        if (exists)
            throw new Exception("SKU đã tồn tại");

        var variant = new ProductVariant
        {
            MaSanPham = dto.MaSanPham,
            MaMauSac = dto.MaMauSac,
            MaKichCo = dto.MaKichCo,
            SKU = dto.SKU,
            SoLuongTon = dto.SoLuongTon,
            DieuChinhGia = dto.DieuChinhGia
        };

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductVariant>> GetByProductAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(x => x.MaSanPham == productId)
            .ToListAsync();
    }
}
