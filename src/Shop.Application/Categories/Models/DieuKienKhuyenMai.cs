namespace Shop.Application.Categories.Models
{
    public class DieuKienKhuyenMai
    {
        public int Ma { get; set; }
        public int MaKhuyenMai { get; set; }
        public string TruongDuLieu { get; set; } = null!; // Ví dụ: 'TongDonHang'
        public string ToanTu { get; set; } = null!;      // Ví dụ: '>='
        public string GiaTri { get; set; } = null!;      // Ví dụ: '500000'
        
        public virtual KhuyenMai? KhuyenMai { get; set; }
    }
}