using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Application.Orders.Models;

[Table("DonHang")]
public class DonHang
{
    [Key]
    public int MaDonHang { get; set; }
    public int MaNguoiDung { get; set; }
    public string TenNguoiNhan { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string DiaChiGiao { get; set; } = string.Empty;
    public decimal TongTien { get; set; }
    public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;
    public DateTime NgayDat { get; set; } = DateTime.Now;
}