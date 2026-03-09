using Shop.Application.Products;

namespace Shop.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Product?> GetProductForUpdateAsync(int productId);
        Task<KhuyenMai?> GetActivePromotionAsync(int productId, DateTime now);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<List<KhuyenMai>> GetActivePromotionsAsync(int productId, DateTime now);
    }
}