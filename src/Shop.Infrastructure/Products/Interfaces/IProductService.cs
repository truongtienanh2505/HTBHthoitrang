using Shop.Application.Products.Dtos;

namespace Shop.Application.Products.DTOs;


public interface IProductService 
{
    Task UpdateAsync(int id, UpdateProductDto dto);
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProductDto dto);
    Task DeleteAsync(int id);
    Task AddVariantAsync(CreateVariantDto dto);
    Task<List<ProductVariant>> GetByProductAsync(int productId);
}
