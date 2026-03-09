using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository; 
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _repository.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    // 1. Lấy tất cả biến thể của một sản phẩm
    [HttpGet("{productId}/variants")]
    public async Task<IActionResult> GetVariants(int productId)
    {
        var product = await _repository.GetByIdAsync(productId);
        if (product == null) return NotFound();
        return Ok(product.BienThes);
    }

// 2. Cập nhật số lượng tồn kho (Nhập hàng / Xuất hàng)
    [HttpPatch("variants/{variantId}/stock")]
    public async Task<IActionResult> UpdateStock(int variantId, [FromBody] int quantityChange)
    {
        var result = await _repository.UpdateStockAsync(variantId, quantityChange);
        if (!result) return BadRequest("Cập nhật kho thất bại (Kho không đủ hoặc sai mã)");
        return Ok(new { message = "Cập nhật tồn kho thành công" });
    }
    // Thêm mới sản phẩm
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        var id = await _repository.CreateAsync(product);
        return CreatedAtAction(nameof(Get), new { id }, product);
    }
    [HttpPost("variants/{variantId}/sell")]
    public async Task<IActionResult> Sell(int variantId, [FromBody] int quantity)
    {
        var result = await _repository.UpdateStock(variantId, quantity);
        if (!result)
        {
        return BadRequest("Không đủ hàng hoặc sai mã biến thể!");
            
        }
        return Ok("Đã cập nhật kho thành công!");
    }   
    [HttpPost("variants/{id}/restock")]
    public async Task<IActionResult> Restock(int id, [FromBody] int quantity)
    {   
        var result = await _repository.UpdateStock(id, -quantity); // Truyền số âm để trừ vào phép trừ (thành cộng)
        if (!result) return BadRequest("Không tìm thấy biến thể!");
        return Ok("Nhập kho thành công!");
    }
    // Cập nhật sản phẩm
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.MaSanPham) return BadRequest("ID không khớp");
        await _repository.UpdateAsync(product);
        return NoContent();
    }

    // Xóa sản phẩm
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}