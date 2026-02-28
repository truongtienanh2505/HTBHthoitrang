namespace Shop.Application.AdminReports;

public interface IAdminReportRepository
{
    /// <summary>
    /// Doanh thu theo ngày cho đơn hàng "Thành công" (MaTrangThai = 4).
    /// Quy ước khoảng thời gian: [from, toExclusive).
    /// </summary>
    Task<List<RevenueByDayDto>> GetRevenueByDayAsync(DateOnly from, DateOnly toExclusive, CancellationToken ct);
}