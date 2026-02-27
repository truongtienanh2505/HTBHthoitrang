using Microsoft.EntityFrameworkCore;
using Shop.Application.Promotions;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Promotions;

public class PromotionCacheRepository : IPromotionCacheRepository
{
    private readonly ShopDbContext _db;
    public PromotionCacheRepository(ShopDbContext db) => _db = db;

    public Task RebuildAsync(CancellationToken ct)
        => _db.Database.ExecuteSqlRawAsync("EXEC dbo.sp_RebuildProductPromotionsCache", ct);

    public async Task<PromotionCacheStatusDto> GetStatusAsync(CancellationToken ct)
    {
        var sql = """
        SELECT
            MAX(CapNhatLuc) AS LastUpdatedUtc,
            CAST(COUNT(1) AS int) AS TotalRows
        FROM dbo.ProductPromotions;
        """;

        var row = await _db.PromotionCacheStatusRows
            .FromSqlRaw(sql)
            .AsNoTracking()
            .SingleAsync(ct);

        return new PromotionCacheStatusDto(row.LastUpdatedUtc, row.TotalRows);
    }
}