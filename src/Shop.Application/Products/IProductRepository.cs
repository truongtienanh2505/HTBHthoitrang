namespace Shop.Application.Products;

public interface IProductRepository
{
    // Lấy danh sách kèm tổng số lượng để phân trang
    Task<(int Total, IEnumerable<object> Items)> GetProductsAsync(string? search, string? cat, string? sort, decimal? minPrice, decimal? maxPrice, int page, int pageSize);
    
    // Lấy chi tiết sản phẩm và biến thể
    Task<object?> GetProductDetailAsync(int id);
}