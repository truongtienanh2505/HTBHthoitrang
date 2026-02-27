namespace Shop.Application.Products;

public class ProductQueryService
{
    private readonly IProductQueryRepository _repo;
    public ProductQueryService(IProductQueryRepository repo) => _repo = repo;

    public Task<List<ProductCardDto>> GetCardsAsync(int skip, int take, CancellationToken ct)
        => _repo.GetProductCardsAsync(skip, take, ct);
}