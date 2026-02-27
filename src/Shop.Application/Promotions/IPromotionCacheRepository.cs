namespace Shop.Application.Promotions;

public interface IPromotionCacheRepository
{
    Task RebuildAsync(CancellationToken ct);
    Task<PromotionCacheStatusDto> GetStatusAsync(CancellationToken ct);
}

public sealed record PromotionCacheStatusDto(DateTime? LastUpdatedUtc, int TotalRows);