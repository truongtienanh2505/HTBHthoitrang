using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products.Interfaces;
using Shop.Application.Products.DTOs;
using Shop.Application.Products.Dtos;

namespace Shop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    // ===============================
    // GET ALL PRODUCTS
    // ===============================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    // ===============================
    // GET PRODUCT BY ID
    // ===============================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm ID = {id}");

        return Ok(product);
    }

    // ===============================
    // CREATE PRODUCT
    // ===============================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _service.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // ===============================
    // UPDATE PRODUCT
    // ===============================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ===============================
    // DELETE PRODUCT
    // ===============================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
