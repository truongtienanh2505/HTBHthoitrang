using Microsoft.AspNetCore.Mvc;
using Shop.Application.Orders.Models;
using Shop.Infrastructure.Orders;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("user/{maNguoiDung:int}")]
    public async Task<IActionResult> GetMyOrders(int maNguoiDung)
    {
        var data = await _orderService.GetMyOrdersAsync(maNguoiDung);
        return Ok(data);
    }

    [HttpGet("{maDonHang:int}/history")]
    public async Task<IActionResult> GetHistory(int maDonHang)
    {
        var data = await _orderService.GetOrderHistoryAsync(maDonHang);
        return Ok(data);
    }

    [HttpPut("{maDonHang:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        int maDonHang,
        [FromQuery] TrangThaiDonHang trangThaiMoi,
        [FromQuery] string ghiChu = "")
    {
        try
        {
            await _orderService.ChangeOrderStatusAsync(maDonHang, trangThaiMoi, ghiChu);
            return Ok(new { message = "Chuyển trạng thái đơn hàng hợp lệ và thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}