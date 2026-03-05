using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly string _connStr;
    public ReviewsController(IConfiguration config) => _connStr = config.GetConnectionString("Default");

    // 1. Lấy danh sách đánh giá của 1 Sản phẩm (Hiển thị ở trang Chi tiết sản phẩm)
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetReviews(int productId)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT d.SoSao, d.NoiDung, d.TaoLuc, n.HoTen 
            FROM DanhGiaSanPham d
            JOIN NguoiDung n ON d.MaNguoiDung = n.MaNguoiDung
            WHERE d.MaSanPham = @ProductId
            ORDER BY d.TaoLuc DESC", conn);
        cmd.Parameters.AddWithValue("@ProductId", productId);
        
        var list = new List<object>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new {
                Rating = reader["SoSao"],
                Content = reader["NoiDung"].ToString(),
                CreatedAt = reader["TaoLuc"],
                ReviewerName = reader["HoTen"].ToString()
            });
        }
        return Ok(list);
    }

    // Class nhận dữ liệu từ Frontend gửi lên
    public class CreateReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; } = "";
    }

    // 2. Thêm Đánh giá mới (Kèm kiểm tra điều kiện mua hàng)
    [HttpPost]
    // [Authorize] // TODO: Bỏ comment khi nhóm làm xong Đăng nhập (JWT)
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        // Tạm thời fix cứng UserID = 1 để test. Sau này đổi thành lấy từ Token
        int userId = 1;

        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();

        // BƯỚC A: Kiểm tra xem User này có đơn hàng nào THÀNH CÔNG (MaTrangThai = 4) chứa sản phẩm này không?
        var cmdCheckOrder = new SqlCommand(@"
            SELECT TOP 1 dh.MaDonHang 
            FROM DonHang dh
            JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
            JOIN BienTheSanPham bt ON ct.MaBienThe = bt.MaBienThe
            WHERE dh.MaNguoiDung = @UserId 
              AND bt.MaSanPham = @ProductId 
              AND dh.MaTrangThai = 4 -- 4: Thành công
              AND NOT EXISTS (
                  -- Đảm bảo đơn hàng này chưa bị đánh giá cho sản phẩm này
                  SELECT 1 FROM DanhGiaSanPham d 
                  WHERE d.MaDonHang = dh.MaDonHang AND d.MaSanPham = @ProductId
              )
            ORDER BY dh.NgayDat DESC", conn);
            
        cmdCheckOrder.Parameters.AddWithValue("@UserId", userId);
        cmdCheckOrder.Parameters.AddWithValue("@ProductId", dto.ProductId);

        var validOrderId = await cmdCheckOrder.ExecuteScalarAsync();

        // Nếu không tìm thấy đơn hàng hợp lệ (Chưa mua, chưa giao xong, hoặc đã đánh giá rồi)
        if (validOrderId == null)
        {
            return BadRequest(new { message = "Bạn phải mua sản phẩm này và nhận hàng thành công mới được đánh giá (Hoặc bạn đã đánh giá đơn hàng này rồi)." });
        }

        // BƯỚC B: Nếu hợp lệ, tiến hành lưu đánh giá vào DB
        var cmdInsert = new SqlCommand(@"
            INSERT INTO DanhGiaSanPham (MaSanPham, MaNguoiDung, MaDonHang, SoSao, NoiDung, TaoLuc)
            VALUES (@ProductId, @UserId, @OrderId, @Rating, @Content, SYSUTCDATETIME())", conn);
            
        cmdInsert.Parameters.AddWithValue("@ProductId", dto.ProductId);
        cmdInsert.Parameters.AddWithValue("@UserId", userId);
        cmdInsert.Parameters.AddWithValue("@OrderId", (int)validOrderId);
        cmdInsert.Parameters.AddWithValue("@Rating", dto.Rating);
        cmdInsert.Parameters.AddWithValue("@Content", dto.Content);

        await cmdInsert.ExecuteNonQueryAsync();

        return Ok(new { message = "Đánh giá sản phẩm thành công!" });
    }
}