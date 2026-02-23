using Shop.Application.Products;

namespace Shop.Application.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> SaveChangesAsync();
    Task<bool> UpdateStockAsync(int variantId, int quantityChange);
    Task<bool> UpdateStockAfterSale(int variantId, int quantitySold);
    Task<bool> UpdateStock(int variantId, int quantity);
}