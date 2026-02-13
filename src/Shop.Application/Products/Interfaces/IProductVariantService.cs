using Shop.Application.Products.DTOs;

namespace Shop.Application.Products.Interfaces;

public interface IProductVariantService
{
    Task AddVariantAsync(CreateVariantDto dto);
    Task<List<ProductVariant>> GetByProductAsync(int productId);
}

