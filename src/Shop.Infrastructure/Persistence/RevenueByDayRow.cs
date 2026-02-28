namespace Shop.Infrastructure.Persistence;

/// <summary>
/// Keyless row for admin revenue report.
/// </summary>
public sealed class RevenueByDayRow
{
    public DateTime Ngay { get; set; }
    public int SoDon { get; set; }
    public decimal? TongTien { get; set; }
    public decimal? TongGiam { get; set; }
    public decimal? TongShip { get; set; }
    public decimal? DoanhThu { get; set; }
}