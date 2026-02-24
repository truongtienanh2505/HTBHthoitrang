using Shop.Infrastructure.Persistence;// Đường dẫn chính xác đến thư mục chứa file context
using Microsoft.EntityFrameworkCore;
namespace Shop.Application.Services
{
    public class PromotionService
    {
        private readonly ShopDbContext _context;

        public PromotionService(ShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logic tính đúng giá sau giảm để phục vụ lưu Snapshot Data (Slide 93)
        /// </summary>
        public async Task<decimal> CalculateDiscountedPrice(int productId, decimal originalPrice)
        {
            var now = DateTime.Now;

            // Tìm khuyến mãi hợp lệ: Đang kích hoạt và còn hạn sử dụng
            var activePromotion = await _context.SanPhamKhuyenMais
                .Include(spkm => spkm.KhuyenMai)
                .Where(spkm => spkm.MaSanPham == productId 
                               && spkm.KhuyenMai.KichHoat == true
                               && spkm.KhuyenMai.NgayBatDau <= now 
                               && spkm.KhuyenMai.NgayKetThuc >= now)
                .Select(spkm => spkm.KhuyenMai)
                .FirstOrDefaultAsync();

            // Nếu không có khuyến mãi, trả về giá gốc ban đầu
            if (activePromotion == null) return originalPrice;

            // Tính toán giảm giá theo PERCENTAGE (Ví dụ: Giảm 20% cho sản phẩm 13)
            if (activePromotion.LoaiGiamGia == "PERCENTAGE")
            {
                var discountAmount = originalPrice * (activePromotion.GiaTriGiam / 100);
                return originalPrice - discountAmount;
            }

            return originalPrice;
        }
    }
}