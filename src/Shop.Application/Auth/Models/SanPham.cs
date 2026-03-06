using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Application.Products.Models;

[Table("SanPham")]
public class SanPham
{
    [Key]
    public int MaSanPham { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string TenSanPham { get; set; } = string.Empty;
    
    public string? MoTa { get; set; }
    
    public decimal GiaGoc { get; set; }
    
    public string? AnhDaiDien { get; set; }
    
    public bool HoatDong { get; set; } = true;
}