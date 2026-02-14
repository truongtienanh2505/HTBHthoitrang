using Shop.Infrastructure.Entities;

public interface IPromotionService
{
    Task<List<Promotion>> GetAllAsync();
    Task<Promotion?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreatePromotionDto dto);
    Task UpdateAsync(int id, UpdatePromotionDto dto);
    Task DeleteAsync(int id);
    Task<decimal> TinhGiaSauGiamAsync(decimal giaGoc, int promotionId);
}
  