using Microsoft.EntityFrameworkCore;
using Xunit;

public class PromotionServiceTest
{
    private ShopDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ShopDbContext(options);
    }

    [Fact]
    public async Task Promotion_Should_Discount_Percentage_Correctly()
    {
        var context = GetDbContext();

        context.Promotions.Add(new Promotion
        {
            TenKhuyenMai = "Test %",
            LoaiGiamGia = PromotionType.Percentage,
            GiaTri = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayKetThuc = DateTime.Now.AddDays(1),
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new PromotionService(context);

        var result = await service.CalculateFinalPriceAsync(0, 100000);

        Assert.Equal(90000, result);
    }

    [Fact]
    public async Task Promotion_Should_Not_Work_When_Expired()
    {
        var context = GetDbContext();

        context.Promotions.Add(new Promotion
        {
            TenKhuyenMai = "Expired",
            LoaiGiamGia = PromotionType.Percentage,
            GiaTri = 10,
            NgayBatDau = DateTime.Now.AddDays(-10),
            NgayKetThuc = DateTime.Now.AddDays(-5),
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new PromotionService(context);

        var result = await service.CalculateFinalPriceAsync(0, 100000);

        Assert.Equal(100000, result);
    }

    [Fact]
    public async Task Promotion_Should_Not_Work_When_Not_Active()
    {
        var context = GetDbContext();

        context.Promotions.Add(new Promotion
        {
            TenKhuyenMai = "Inactive",
            LoaiGiamGia = PromotionType.FixedAmount,
            GiaTri = 50000,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayKetThuc = DateTime.Now.AddDays(1),
            IsActive = false
        });

        await context.SaveChangesAsync();

        var service = new PromotionService(context);

        var result = await service.CalculateFinalPriceAsync(0, 100000);

        Assert.Equal(100000, result);
    }

    [Fact]
    public async Task Promotion_Should_Not_Return_Negative_Price()
    {
        var context = GetDbContext();

        context.Promotions.Add(new Promotion
        {
            TenKhuyenMai = "Big Discount",
            LoaiGiamGia = PromotionType.FixedAmount,
            GiaTri = 200000,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayKetThuc = DateTime.Now.AddDays(1),
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new PromotionService(context);

        var result = await service.CalculateFinalPriceAsync(0, 100000);

        Assert.Equal(0, result);
    }
}
