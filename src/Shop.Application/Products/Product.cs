using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shop.Application.Categories.Models;

namespace Shop.Application.Products
{
    [Table("SanPham")]
    public class Product
    {
        [Key]

        public int MaSanPham { get; set; }

        [Required, MaxLength(200)]
        public string TenSanPham { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Slug { get; set; } = null!;

        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaGoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int MaDanhMuc { get; set; }

        public string? AnhDaiDien { get; set; }

        public bool HoatDong { get; set; } = true;

        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;
        public DateTime? CapNhatLuc { get; set; }

        [ForeignKey("MaDanhMuc")]
        public virtual DanhMuc? DanhMuc { get; set; }

        public virtual ICollection<ProductVariant>? BienThes { get; set; } = new List<ProductVariant>();
    }
}