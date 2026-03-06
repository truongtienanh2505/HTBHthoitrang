namespace Shop.Application.Reviews;

public interface IReviewRepository
{
    Task<IEnumerable<object>> GetReviewsByProductAsync(int productId);
    Task<bool> HasPurchasedAsync(int userId, int productId);
    Task CreateReviewAsync(int userId, ReviewDto dto);
}