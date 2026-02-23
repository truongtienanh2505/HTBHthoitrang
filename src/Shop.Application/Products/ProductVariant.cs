using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Shop.Application.Products
{
    [Table("BienTheSanPham")]
    public class ProductVariant
    {
        [Key]
        public int MaBienThe { get; set; }
         public int MaSanPham { get; set; }
        public string? KichThuoc { get; set; }
        public string? MauSac { get; set; }
         
       
        public int MaMauSac { get; set; }
        public int MaKichCo { get; set; }

        [Required, MaxLength(50)]
        public string Sku { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public decimal GiaBienThe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DieuChinhGia { get; set; }

        public bool HoatDong { get; set; } = true;

        [ForeignKey("MaSanPham")]

        [JsonIgnore]
        public virtual Product? Product { get; set; } = null!;
    }
}