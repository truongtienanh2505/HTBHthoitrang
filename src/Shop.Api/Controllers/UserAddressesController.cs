using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.UserAddresses;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/user-addresses")]
[Authorize]
public class UserAddressesController : ControllerBase
{
    private readonly UserAddressService _service;

    public UserAddressesController(UserAddressService service)
    {
        _service = service;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;

        return int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token không hợp lệ hoặc bị thiếu!" });

        var list = await _service.GetAddressesAsync(userId.Value);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] AddressDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token không hợp lệ hoặc bị thiếu!" });

        try
        {
            await _service.CreateAddressAsync(userId.Value, dto);
            return Ok(new { message = "Thêm địa chỉ thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token không hợp lệ hoặc bị thiếu!" });

        try
        {
            bool success = await _service.UpdateAddressAsync(id, userId.Value, dto);
            if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ của bạn!" });

            return Ok(new { message = "Cập nhật địa chỉ thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { message = "Token không hợp lệ hoặc bị thiếu!" });

        bool success = await _service.DeleteAddressAsync(id, userId.Value);
        if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ để xóa!" });

        return Ok(new { message = "Xóa địa chỉ thành công!" });
    }
}