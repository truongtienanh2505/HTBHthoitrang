using Microsoft.EntityFrameworkCore;
using Shop.Application.Products;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Products;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _context;

    public ProductRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.BienThes)
            .FirstOrDefaultAsync(p => p.MaSanPham == id);
    }

    public async Task<int> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product.MaSanPham;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync()) > 0;
    }

    public async Task<bool> UpdateStockAsync(int variantId, int quantityChange)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null) return false;

    // Logic: Nếu quantityChange âm (bán hàng) thì trừ kho, dương (nhập hàng) thì cộng kho
        variant.SoLuongTon += quantityChange;

        if (variant.SoLuongTon < 0) return false; // Không cho phép kho âm

        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateStockAfterSale(int variantId, int quantitySold)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null || variant.SoLuongTon < quantitySold) return false;

        variant.SoLuongTon -= quantitySold; // Trừ số lượng trong kho
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateStock(int variantId, int quantity)
    {
    // Tìm biến thể trong Database
        var variant = await _context.BienTheSanPhams
        .FirstOrDefaultAsync(v => v.MaBienThe == variantId);

        if (variant == null || variant.SoLuongTon < quantity)
        {
            return false; // Trả về false nếu không tìm thấy hoặc không đủ hàng
        }   

    // Thực hiện trừ kho
        variant.SoLuongTon -= quantity;
    
    // Lưu thay đổi xuống Database
        return await _context.SaveChangesAsync() > 0;
        
    }   
    
}