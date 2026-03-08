using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly string _connStr;
    public UsersController(IConfiguration config) => _connStr = config.GetConnectionString("Default");

    [HttpGet("me")]
    [Authorize] // BẬT BẢO MẬT: Phải có Token mới được vào!
    public async Task<IActionResult> GetMe()
    {
        // 1. Lấy ID từ JWT Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized(new { message = "Token không hợp lệ hoặc bị thiếu!" });
        
        int userId = int.Parse(userIdClaim.Value);

        // 2. Lấy thông tin từ DB dựa vào ID vừa giải mã
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand("SELECT MaNguoiDung, HoTen, Email, SoDienThoai FROM NguoiDung WHERE MaNguoiDung = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", userId);
        
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return NotFound(new { message = "Không tìm thấy người dùng" });

        var user = new {
            MaNguoiDung = reader["MaNguoiDung"],
            HoTen = reader["HoTen"].ToString(),
            Email = reader["Email"].ToString(),
            SoDienThoai = reader["SoDienThoai"].ToString()
        };
        
        return Ok(user);
    }
}