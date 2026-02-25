using Shop.Application.Products;

namespace Shop.Application.Interfaces
{
    public interface IPromotionRepository
    {
        Task<List<KhuyenMai>> GetActivePromotionsAsync(int productId, DateTime now);
    }
}