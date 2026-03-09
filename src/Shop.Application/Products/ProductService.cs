namespace Shop.Application.Products;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> GetProductsAsync(string? search, string? cat, string? sort, decimal? minPrice, decimal? maxPrice, int page, int pageSize)
    {
        var result = await _repository.GetProductsAsync(search, cat, sort, minPrice, maxPrice, page, pageSize);
        // Trả về đúng format mà file products.js của bạn đang mong đợi
        return new { total = result.Total, items = result.Items };
    }

    public async Task<object?> GetProductDetailAsync(int id)
    {
        return await _repository.GetProductDetailAsync(id);
    }
}