using Microsoft.AspNetCore.Mvc;
using Shop.Application.Auth;
using System;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 1. Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                // Trả về HTTP 400 nếu lỗi (ví dụ: email đã tồn tại)
                return BadRequest(new { Message = ex.Message }); 
            }
        }

        // 2. Đăng nhập bằng tài khoản/mật khẩu thông thường (JWT)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var token = await _authService.LoginAsync(request);
                return Ok(new { Token = token, Message = "Đăng nhập thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 3. Đăng nhập/Đăng ký bằng Google (Gộp tài khoản)
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDto request)
        {
            try
            {
                var token = await _authService.GoogleLoginAsync(request.Token);
                return Ok(new { Token = token, Message = "Đăng nhập Google thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 4. Xóa mềm (Soft Delete - IsDeleted/BiKhoa)
        [HttpDelete("soft-delete/{userId}")]
        public async Task<IActionResult> SoftDelete(int userId)
        {
            try
            {
                var result = await _authService.SoftDeleteUserAsync(userId);
                if (!result) return NotFound(new { Message = "Không tìm thấy người dùng." });

                return Ok(new { Message = "Đã khóa/xóa tài khoản thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}