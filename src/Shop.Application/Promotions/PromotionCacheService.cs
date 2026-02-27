namespace Shop.Application.Promotions;

public class PromotionCacheService
{
    private readonly IPromotionCacheRepository _repo;
    public PromotionCacheService(IPromotionCacheRepository repo) => _repo = repo;

    public Task RebuildAsync(CancellationToken ct) => _repo.RebuildAsync(ct);
    public Task<PromotionCacheStatusDto> StatusAsync(CancellationToken ct) => _repo.GetStatusAsync(ct);
}