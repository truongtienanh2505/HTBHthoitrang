using Shop.Application.Interfaces; // Nhận diện interface mới
namespace Shop.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateOrder(int productId, int quantity)
        {
            var now = DateTime.Now;

            // Xử lý Race Condition thông qua Repository
            await _repository.BeginTransactionAsync();

            try
            {
                // Lấy sản phẩm và khóa hàng (UPDLOCK logic nằm trong Repository)
                var product = await _repository.GetProductForUpdateAsync(productId);

                if (product == null || product.Stock < quantity)
                    throw new Exception("Sản phẩm không tồn tại hoặc đã hết hàng");

                // Tính toán Promotion (Logic Tuần 2)
                var activePromotion = await _repository.GetActivePromotionAsync(productId, now);

                decimal finalPrice = product.Price;

                if (activePromotion != null && activePromotion.LoaiGiamGia == "PERCENTAGE")
                {
                    // Tính đúng giá sau giảm (Snapshot Data)
                    finalPrice -= product.Price * (activePromotion.GiaTriGiam / 100);
                }

                // Cập nhật tồn kho
                product.Stock -= quantity;

                await _repository.SaveChangesAsync();
                await _repository.CommitAsync();
                // --- Logic Tuần 2: Xử lý nhiều khuyến mãi cùng lúc ---

                var promotions = await _repository.GetActivePromotionsAsync(productId, now);
                if (promotions != null && promotions.Any())
                {
                var bestPromotion = promotions
                .OrderByDescending(p => p.LoaiGiamGia == "PERCENTAGE" ? product.Price * (p.GiaTriGiam / 100) : p.GiaTriGiam)
                .FirstOrDefault();

                if (bestPromotion != null && bestPromotion.LoaiGiamGia == "PERCENTAGE")
                {
                finalPrice -= product.Price * (bestPromotion.GiaTriGiam / 100);
                }
                }
// ------------------------------------------------------
            }
            catch
            {
                await _repository.RollbackAsync();
                throw;
            }
        }
    }
}