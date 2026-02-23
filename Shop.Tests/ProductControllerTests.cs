using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductsController _controller;

    public ProductControllerTests()
    {
        // 1. Giả lập Repository
        _mockRepo = new Mock<IProductRepository>();
        
        // 2. Truyền Mock vào Controller (Dependency Injection)
        _controller = new ProductsController(_mockRepo.Object);
    }

    [Fact]
    public async Task Sell_ReturnsOk_WhenStockIsUpdated()
    {
        // Arrange: Giả lập khi gọi UpdateStock với số lượng 10 sẽ thành công
        _mockRepo.Setup(repo => repo.UpdateStock(It.IsAny<int>(), 10))
                 .ReturnsAsync(true);

        // Act: Thực hiện hành động bán 10 sản phẩm
        var result = await _controller.Sell(5, 10);

        // Assert: Mong đợi kết quả trả về là Ok
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Sell_ReturnsBadRequest_WhenStockIsNotEnough()
    {
        // Arrange: Giả lập trường hợp không đủ hàng
        _mockRepo.Setup(repo => repo.UpdateStock(It.IsAny<int>(), 999))
                 .ReturnsAsync(false);

        // Act: Thử bán số lượng cực lớn
        var result = await _controller.Sell(5, 999);

        // Assert: Mong đợi trả về lỗi 400 Bad Request
        Assert.IsType<BadRequestObjectResult>(result);
    }
}