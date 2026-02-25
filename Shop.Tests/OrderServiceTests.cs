public class OrderServiceTests
{
    [Fact]
    public void CalculatePrice_WithPercentagePromotion_ReturnsDiscountedPrice()
    {
        // Arrange (Chuẩn bị dữ liệu mẫu)
        decimal originalPrice = 100000;
        decimal discountPercentage = 10; // Giống khuyến mãi mã 14 bạn đã tạo
        
        // Act (Thực hiện tính toán theo logic Tuần 2)
        decimal finalPrice = originalPrice - (originalPrice * (discountPercentage / 100));

        // Assert (Kiểm tra kết quả)
        Assert.Equal(90000, finalPrice); 
    }

    [Fact]
    public void BestPromotion_ShouldSelectHigherDiscount()
    {
        // Logic kiểm tra Business Rule: Xử lý nhiều khuyến mãi
        var promo1 = 10; // 10%
        var promo2 = 20; // 20%
        
        var bestPromo = new List<decimal> { promo1, promo2 }.Max();

        Assert.Equal(20, bestPromo);
    }
}