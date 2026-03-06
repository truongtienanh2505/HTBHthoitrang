using System;

namespace Shop.Application.Auth.Models
{
    public class NguoiDung
    {
        public int MaNguoiDung { get; set; }
        public string? TenDangNhap { get; set; }
        public string? MatKhauHash { get; set; }
        public string Email { get; set; } = null!;
        public string? GoogleId { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string HoTen { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }
        public int MaVaiTro { get; set; }
        public bool BiKhoa { get; set; }
        public DateTime TaoLuc { get; set; }
        public DateTime? CapNhatLuc { get; set; }
    }
}