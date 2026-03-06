using Microsoft.AspNetCore.Mvc;
using Shop.Application.Orders.Models;
using Shop.Infrastructure.Orders;

namespace Shop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    public OrderController(OrderService orderService) { _orderService = orderService; }

    [HttpGet("user/{maNguoiDung}")]
    public async Task<IActionResult> GetMyOrders(int maNguoiDung)
    {
        return Ok(await _orderService.GetMyOrdersAsync(maNguoiDung));
    }

    [HttpGet("{maDonHang}/history")]
    public async Task<IActionResult> GetHistory(int maDonHang)
    {
        return Ok(await _orderService.GetOrderHistoryAsync(maDonHang));
    }

    [HttpPut("{maDonHang}/status")]
    public async Task<IActionResult> ChangeStatus(int maDonHang, [FromQuery] TrangThaiDonHang trangThaiMoi, [FromQuery] string ghiChu = "")
    {
        try
        {
            await _orderService.ChangeOrderStatusAsync(maDonHang, trangThaiMoi, ghiChu);
            return Ok(new { message = "Chuyển trạng thái đơn hàng hợp lệ & thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}