using Microsoft.AspNetCore.Mvc;
using Shop.Application.Promotions;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/promotion-cache")]
public class PromotionCacheController : ControllerBase
{
    private readonly PromotionCacheService _svc;
    public PromotionCacheController(PromotionCacheService svc) => _svc = svc;

    [HttpGet("status")]
    public Task<PromotionCacheStatusDto> Status(CancellationToken ct)
        => _svc.StatusAsync(ct);

    [HttpPost("rebuild")]
    public async Task<ActionResult> Rebuild(CancellationToken ct)
    {
        await _svc.RebuildAsync(ct);
        return NoContent();
    }
}