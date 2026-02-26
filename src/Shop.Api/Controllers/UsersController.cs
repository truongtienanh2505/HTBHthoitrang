using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly string _connStr;
    public UsersController(IConfiguration config) => _connStr = config.GetConnectionString("Default");

    [HttpGet("me")]
    // [Authorize] // TODO CHO NHÓM: Bỏ comment dòng này khi đã cài đặt JWT Authentication
    public async Task<IActionResult> GetMe()
    {
        // ==========================================
        // [TODO CHO NHÓM]: LẤY ID TỪ JWT TOKEN
        // ==========================================
        // Khi làm xong chức năng Đăng nhập, hãy mở comment 2 dòng dưới đây
        // và xóa dòng "int userId = 1;" đi nhé!
        
        // var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        // int userId = int.Parse(userIdClaim.Value);
        
        int userId = 1; // Tạm thời Fix cứng ID = 1 để test giao diện trước

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