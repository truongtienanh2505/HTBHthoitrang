using Shop.Application.Categories.Models;
using Shop.Application.Products; // Chỉ định đúng thư mục chứa file Product.cs của bạn
namespace Shop.Application.Categories.Models
{
    public class SanPhamKhuyenMai
    {
        public int Ma { get; set; } // Khóa chính
        public int MaSanPham { get; set; }
        public int MaKhuyenMai { get; set; }

        // Navigation properties
        public virtual Product? SanPham { get; set; }
        public virtual KhuyenMai? KhuyenMai { get; set; }
    }
}