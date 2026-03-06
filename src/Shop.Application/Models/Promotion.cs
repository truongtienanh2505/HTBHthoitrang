using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Application.Models
{
    [Table("KhuyenMai")]
    public class Promotion
    {
        [Key]
        public int MaKhuyenMai { get; set; }

        public string TenKhuyenMai { get; set; }

        public string LoaiGiamGia { get; set; }

        public decimal GiaTriGiam { get; set; }

        public decimal? GiamToiDa { get; set; }

        [Column("BatDau")]
        public DateTime BatDau { get; set; }

        [Column("KetThuc")]
        public DateTime KetThuc { get; set; }

        public bool KichHoat { get; set; }
    }
}