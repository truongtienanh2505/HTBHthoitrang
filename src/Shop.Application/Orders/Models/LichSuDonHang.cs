using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Application.Orders.Models;

[Table("LichSuDonHang")]
public class LichSuDonHang
{
    [Key]
    public int MaLichSu { get; set; }
    public int MaDonHang { get; set; }
    public TrangThaiDonHang TrangThaiCu { get; set; }
    public TrangThaiDonHang TrangThaiMoi { get; set; }
    public string GhiChu { get; set; } = string.Empty;
    public DateTime NgayThayDoi { get; set; } = DateTime.Now;
}