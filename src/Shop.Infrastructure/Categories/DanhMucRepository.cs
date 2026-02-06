using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories;
using Shop.Infrastructure.Persistence;
using Shop.Application.Categories.Models;


namespace Shop.Infrastructure.Categories;

public class DanhMucRepository : IDanhMucRepository
{
    private readonly ShopDbContext _db;

    public DanhMucRepository(ShopDbContext db) => _db = db;

    public Task<List<DanhMuc>> GetAllAsync(bool includeInactive, CancellationToken ct)
    {
        var q = _db.DanhMucs.AsNoTracking();
        if (!includeInactive) q = q.Where(x => x.HoatDong);
        return q.OrderBy(x => x.MaDanhMuc).ToListAsync(ct);
    }

    public Task<DanhMuc?> GetByIdAsync(int id, CancellationToken ct)
        => _db.DanhMucs.FirstOrDefaultAsync(x => x.MaDanhMuc == id, ct);

    public async Task<int> CreateAsync(DanhMuc entity, CancellationToken ct)
    {
        _db.DanhMucs.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.MaDanhMuc;
    }

    public Task UpdateAsync(DanhMuc entity, CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(DanhMuc entity, CancellationToken ct)
    {
        _db.DanhMucs.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<DanhMucTreeRow>> GetTreeRowsAsync(CancellationToken ct)
    {
        // CTE đệ quy đúng yêu cầu tuần 1 M4
        var sql = """
        WITH cte AS (
            SELECT 
                MaDanhMuc, TenDanhMuc, Slug, MaDanhMucCha, HoatDong,
                0 AS [Level],
                CAST(RIGHT('000000' + CAST(MaDanhMuc AS varchar(6)), 6) AS varchar(max)) AS SortPath
            FROM dbo.DanhMuc
            WHERE MaDanhMucCha IS NULL

            UNION ALL

            SELECT
                d.MaDanhMuc, d.TenDanhMuc, d.Slug, d.MaDanhMucCha, d.HoatDong,
                cte.[Level] + 1,
                CAST(cte.SortPath + '/' + RIGHT('000000' + CAST(d.MaDanhMuc AS varchar(6)), 6) AS varchar(max))
            FROM dbo.DanhMuc d
            JOIN cte ON d.MaDanhMucCha = cte.MaDanhMuc
        )
        SELECT MaDanhMuc, TenDanhMuc, Slug, MaDanhMucCha, HoatDong, [Level], SortPath
        FROM cte
        ORDER BY SortPath;
        """;

        return _db.DanhMucTreeRows.FromSqlRaw(sql).AsNoTracking().ToListAsync(ct);
    }
}
