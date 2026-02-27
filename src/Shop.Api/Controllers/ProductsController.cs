using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductQueryService _svc;
    public ProductsController(ProductQueryService svc) => _svc = svc;

    [HttpGet("cards")]
    public Task<List<ProductCardDto>> Cards([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        => _svc.GetCardsAsync(skip, take, ct);
}