using Microsoft.AspNetCore.Mvc;
using Shop.Application.UserAddresses;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/user-addresses")]
public class UserAddressesController : ControllerBase
{
    private readonly UserAddressService _service;

    public UserAddressesController(UserAddressService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        int userId = 1; // Tạm fix cứng
        var list = await _service.GetAddressesAsync(userId);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] AddressDto dto)
    {
        int userId = 1;
        try {
            await _service.CreateAddressAsync(userId, dto);
            return Ok(new { message = "Thêm địa chỉ thành công!" });
        } catch(Exception ex) {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto dto)
    {
        int userId = 1;
        try {
            bool success = await _service.UpdateAddressAsync(id, userId, dto);
            if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ của bạn!" });
            return Ok(new { message = "Cập nhật địa chỉ thành công!" });
        } catch(Exception ex) {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        int userId = 1;
        bool success = await _service.DeleteAddressAsync(id, userId);
        if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ để xóa!" });
        return Ok(new { message = "Xóa địa chỉ thành công!" });
    }
}