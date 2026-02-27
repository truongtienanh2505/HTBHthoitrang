namespace Shop.Application.Products;

public interface IProductQueryRepository
{
    Task<List<ProductCardDto>> GetProductCardsAsync(int skip, int take, CancellationToken ct);
}