using Microsoft.EntityFrameworkCore;
using Shop.Application.AdminReports;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.AdminReports;

public sealed class AdminReportRepository : IAdminReportRepository
{
    private readonly ShopDbContext _db;
    public AdminReportRepository(ShopDbContext db) => _db = db;

    public async Task<List<RevenueByDayDto>> GetRevenueByDayAsync(DateOnly from, DateOnly toExclusive, CancellationToken ct)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = toExclusive.ToDateTime(TimeOnly.MinValue);

        var sql = """
        SELECT
          CAST(dh.NgayDat AS date) AS Ngay,
          CAST(COUNT(*) AS int) AS SoDon,
          SUM(dh.TongTien) AS TongTien,
          SUM(dh.TienGiam) AS TongGiam,
          SUM(dh.PhiShip) AS TongShip,
          SUM(dh.ThanhTien) AS DoanhThu
        FROM dbo.DonHang dh
        WHERE dh.MaTrangThai = 4
          AND dh.NgayDat >= {0}
          AND dh.NgayDat <  {1}
        GROUP BY CAST(dh.NgayDat AS date)
        ORDER BY Ngay
        """;

        var rows = await _db.RevenueByDayRow
            .FromSqlRaw(sql, fromDt, toDt)
            .AsNoTracking()
            .ToListAsync(ct);

        return rows.Select(r => new RevenueByDayDto(
            DateOnly.FromDateTime(r.Ngay),
            r.SoDon,
            r.TongTien ?? 0m,
            r.TongGiam ?? 0m,
            r.TongShip ?? 0m,
            r.DoanhThu ?? 0m
        )).ToList();
    }
}