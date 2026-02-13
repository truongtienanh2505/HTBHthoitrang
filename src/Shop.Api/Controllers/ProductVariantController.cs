using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products.DTOs;
using Shop.Application.Products.Interfaces;
using Shop.Infrastructure.Entities;

namespace Shop.Api.Controllers;

[Route("api/variants")]
[ApiController]
public class ProductVariantController : ControllerBase
{
    private readonly IProductVariantService _service;

    public ProductVariantController(IProductVariantService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateVariantDto dto)
    {
        await _service.AddVariantAsync(dto);
        return Ok("Thêm biến thể thành công");
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var data = await _service.GetByProductAsync(productId);
        return Ok(data);
    }
}
