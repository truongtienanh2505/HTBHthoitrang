using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

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
        var result = await _service.GetProductsAsync(search, cat, sort, minPrice, maxPrice, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var data = await _service.GetProductDetailAsync(id);
        if (data == null) return NotFound(new { message = "Sản phẩm không tồn tại!" });
        return Ok(data);
    }
}