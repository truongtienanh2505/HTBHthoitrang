namespace Shop.Application.AdminReports;

public class AdminReportService
{
    private readonly IAdminReportRepository _repo;
    public AdminReportService(IAdminReportRepository repo) => _repo = repo;

    public Task<List<RevenueByDayDto>> RevenueByDayAsync(DateOnly from, DateOnly toExclusive, CancellationToken ct)
        => _repo.GetRevenueByDayAsync(from, toExclusive, ct);
}