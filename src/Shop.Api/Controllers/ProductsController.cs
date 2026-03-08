using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductQueryService _queryService;
    private readonly ProductService _productService;

    public ProductsController(ProductQueryService queryService, ProductService productService)
    {
        _queryService = queryService;
        _productService = productService;
    }

    // Bản gốc
    [HttpGet("cards")]
    public Task<List<ProductCardDto>> Cards(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
        => _queryService.GetCardsAsync(skip, take, ct);

    // Tiến Anh
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? search,
        [FromQuery] string? cat,
        [FromQuery] string? sort,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var result = await _productService.GetProductsAsync(search, cat, sort, minPrice, maxPrice, page, pageSize);
        return Ok(result);
    }

    // Tiến Anh
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var data = await _productService.GetProductDetailAsync(id);
        if (data == null)
            return NotFound(new { message = "Sản phẩm không tồn tại!" });

        return Ok(data);
    }
}