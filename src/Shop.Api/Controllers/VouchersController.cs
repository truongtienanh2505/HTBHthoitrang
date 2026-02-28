using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Vouchers;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/vouchers")]
public sealed class VouchersController : ControllerBase
{
    private readonly VoucherService _svc;
    public VouchersController(VoucherService svc) => _svc = svc;

    [HttpPost("preview")]
    [Authorize]
    public async Task<ActionResult<VoucherApplyResultDto>> Preview([FromBody] VoucherApplyRequest req, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return await _svc.PreviewAsync(userId.Value, req, ct);
    }

    /// <summary>
    /// Consume voucher (set DaDung = 1) theo transaction + lock để chống race.
    /// </summary>
    [HttpPost("consume")]
    [Authorize]
    public async Task<ActionResult<VoucherApplyResultDto>> Consume([FromBody] VoucherApplyRequest req, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        return await _svc.ConsumeAsync(userId.Value, req, ct);
    }

    private int? GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (int.TryParse(raw, out var id) && id > 0) return id;
        return null;
    }
}