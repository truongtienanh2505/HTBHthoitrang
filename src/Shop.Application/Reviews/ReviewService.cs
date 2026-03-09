namespace Shop.Application.Reviews;

public class ReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<object>> GetReviewsByProductAsync(int productId)
    {
        return await _repository.GetReviewsByProductAsync(productId);
    }

    public async Task<(bool Success, string Message)> SubmitReviewAsync(int userId, ReviewDto dto)
    {
        // Kiểm tra xem User đã mua sản phẩm này chưa (VD: MaTrangThai = 4 là Đã giao)
        bool hasPurchased = await _repository.HasPurchasedAsync(userId, dto.ProductId);
        if (!hasPurchased) 
            return (false, "Bạn phải mua và nhận sản phẩm này thành công mới được đánh giá!");

        await _repository.CreateReviewAsync(userId, dto);
        return (true, "Đánh giá thành công!");
    }
}