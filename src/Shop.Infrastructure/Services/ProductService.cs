using Shop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Products.DTOs;
using Shop.Application.Products.Dtos;

public class ProductService : IProductService
{
    private readonly ShopDbContext _context;

    public ProductService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.DanhMuc)
            .Select(p => new ProductDto
            {
                MaSanPham = p.MaSanPham,
                TenSanPham = p.TenSanPham,
                GiaGoc = p.GiaGoc,
                TenDanhMuc = p.DanhMuc.TenDanhMuc
            })
            .ToListAsync();
            
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.DanhMuc)
            .Where(p => p.MaSanPham == id)
            .Select(p => new ProductDto
            {
                MaSanPham = p.MaSanPham,
                TenSanPham = p.TenSanPham,
                GiaGoc = p.GiaGoc,
                TenDanhMuc = p.DanhMuc.TenDanhMuc
            })
            .FirstOrDefaultAsync();
    }
    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<int> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            TenSanPham = dto.TenSanPham,
            Slug = dto.Slug,
            MoTa = dto.MoTa,
            GiaGoc = dto.GiaGoc,
            MaDanhMuc = dto.MaDanhMuc
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.MaSanPham;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            throw new Exception("Không tìm thấy sản phẩm");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
    var product = await _context.Products.FindAsync(id);

    if (product == null)
        throw new Exception("Không tìm thấy sản phẩm");

    product.TenSanPham = dto.TenSanPham;
    product.Slug = dto.Slug;
    product.MoTa = dto.MoTa;
    product.GiaGoc = dto.GiaGoc;
    product.MaDanhMuc = dto.MaDanhMuc;
    product.HoatDong = dto.HoatDong;

    await _context.SaveChangesAsync();
    }

}
