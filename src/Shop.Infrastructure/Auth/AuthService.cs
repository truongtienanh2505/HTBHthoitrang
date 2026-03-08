using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shop.Application.Auth;
using Shop.Application.Auth.Models;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ShopDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto request)
        {
            var exists = await _context.NguoiDungs.AnyAsync(u => u.Email == request.Email);
            if (exists) throw new Exception("Email đã tồn tại.");

            var user = new NguoiDung
            {
                Email = request.Email,
                HoTen = request.FullName,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                MaVaiTro = 2,
                BiKhoa = false,
                TaoLuc = DateTime.UtcNow
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();
            return "Đăng ký thành công!";
        }

        public async Task<string> LoginAsync(LoginDto request)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || string.IsNullOrEmpty(user.MatKhauHash) || !BCrypt.Net.BCrypt.Verify(request.Password, user.MatKhauHash))
                throw new Exception("Sai email hoặc mật khẩu.");

            if (user.BiKhoa)
                throw new Exception("Tài khoản của bạn đã bị khóa hoặc bị xóa.");

            return GenerateJwtToken(user);
        }

        public async Task<string> GoogleLoginAsync(string credentialToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _config["Authentication:Google:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credentialToken, settings);

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user != null)
            {
                if (user.BiKhoa)
                    throw new Exception("Tài khoản đã bị khóa.");

                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = payload.Subject;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                user = new NguoiDung
                {
                    Email = payload.Email,
                    GoogleId = payload.Subject,
                    HoTen = payload.Name,
                    AnhDaiDien = payload.Picture,
                    MaVaiTro = 2,
                    BiKhoa = false,
                    TaoLuc = DateTime.UtcNow
                };

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();
            }

            return GenerateJwtToken(user);
        }

        public async Task<bool> SoftDeleteUserAsync(int userId)
        {
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null) return false;

            user.BiKhoa = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(NguoiDung user)
        {
            var secretKey = _config["JwtSettings:Secret"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new Exception("Thiếu cấu hình JWT Secret.");

            var roleName = user.MaVaiTro == 1 ? "Admin" : "User";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),

                // Giữ thêm claim cũ để tránh code nào đó đang đọc kiểu custom bị gãy
                new Claim("UserId", user.MaNguoiDung.ToString()),
                new Claim("Role", roleName),
                new Claim("MaVaiTro", user.MaVaiTro.ToString()),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryMinutes = double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "1440");

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}