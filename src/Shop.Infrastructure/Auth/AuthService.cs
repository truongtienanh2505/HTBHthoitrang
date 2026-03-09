using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; 
using Shop.Application.Auth; 
using Shop.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore;
using Shop.Application.Auth.Models;


namespace Shop.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ShopDbContext _context; // Đã đổi chuẩn theo cấu trúc của bạn
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
                MaVaiTro = 2, // Role 2 theo Default constraint trong database
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
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _config["Authentication:Google:ClientId"] }
            };
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentialToken, settings);

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user != null)
            {
                if (user.BiKhoa) throw new Exception("Tài khoản đã bị khóa.");

                // Gộp tài khoản nếu chưa có GoogleId
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = payload.Subject;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Tạo mới nếu chưa tồn tại
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

            user.BiKhoa = true; // Soft Delete
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(NguoiDung user)
        {
            var secretKey = _config["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secretKey)) throw new Exception("Thiếu cấu hình JWT Secret.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.MaNguoiDung.ToString()),
                new Claim("Role", user.MaVaiTro.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryMinutes = double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "1440");

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}