using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.AdminReports;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/admin/reports")]
public sealed class AdminReportsController : ControllerBase
{
    private readonly AdminReportService _svc;
    public AdminReportsController(AdminReportService svc) => _svc = svc;

    /// <summary>
    /// Doanh thu theo ngày (đơn "Thành công" - MaTrangThai = 4).
    /// Quy ước: [from, to) (to là ngày kết thúc KHÔNG bao gồm).
    /// Ví dụ: from=2026-02-01&to=2026-03-01
    /// </summary>
    [HttpGet("revenue-by-day")]
    [Authorize(Roles = "Admin")]
    public Task<List<RevenueByDayDto>> RevenueByDay(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken ct = default)
        => _svc.RevenueByDayAsync(from, to, ct);
}