using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Application.Reviews;

namespace Shop.Infrastructure.Reviews;

public class ReviewRepository : IReviewRepository
{
    private readonly string _connStr;
    public ReviewRepository(IConfiguration config) => _connStr = config.GetConnectionString("Default")!;

    public async Task<IEnumerable<object>> GetReviewsByProductAsync(int productId)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT d.SoSao AS Rating, d.NoiDung AS Content, d.TaoLuc AS CreatedAt, n.HoTen AS ReviewerName
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
                ReviewerName = reader["ReviewerName"].ToString(),
                Rating = reader["Rating"],
                Content = reader["Content"].ToString(),
                CreatedAt = reader["CreatedAt"]
            });
        }
        return list;
    }

    // Đổi logic: Trả về MaDonHang thay vì chỉ true/false
    public async Task<bool> HasPurchasedAsync(int userId, int productId)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT TOP 1 d.MaDonHang 
            FROM ChiTietDonHang c
            JOIN DonHang d ON c.MaDonHang = d.MaDonHang
            WHERE d.MaNguoiDung = @UserId AND c.MaSanPham = @ProductId AND d.MaTrangThai = 4
            ORDER BY d.NgayDat DESC", conn);
        
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@ProductId", productId);

        var result = await cmd.ExecuteScalarAsync();
        if (result == null) return false;

        // Lưu tạm MaDonHang vào đâu đó, hoặc truyền vào Insert. 
        // Để giữ Interface cũ, ta sửa hàm CreateReviewAsync bên dưới tự truy vấn lại MaDonHang cho nhanh.
        return true; 
    }

    public async Task CreateReviewAsync(int userId, ReviewDto dto)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        // Tìm lại MaDonHang hợp lệ nhất để lưu
        var cmdGetOrder = new SqlCommand(@"
            SELECT TOP 1 d.MaDonHang FROM ChiTietDonHang c
            JOIN DonHang d ON c.MaDonHang = d.MaDonHang
            WHERE d.MaNguoiDung = @UserId AND c.MaSanPham = @ProductId AND d.MaTrangThai = 4", conn);
        cmdGetOrder.Parameters.AddWithValue("@UserId", userId);
        cmdGetOrder.Parameters.AddWithValue("@ProductId", dto.ProductId);
        int orderId = (int)await cmdGetOrder.ExecuteScalarAsync();

        // Chèn vào bảng DanhGiaSanPham
        var cmd = new SqlCommand(@"
            INSERT INTO DanhGiaSanPham (MaSanPham, MaNguoiDung, MaDonHang, SoSao, NoiDung, TaoLuc)
            VALUES (@ProductId, @UserId, @OrderId, @Rating, @Content, SYSUTCDATETIME())", conn);
            
        cmd.Parameters.AddWithValue("@ProductId", dto.ProductId);
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@Rating", dto.Rating);
        cmd.Parameters.AddWithValue("@Content", dto.Content);
        
        await cmd.ExecuteNonQueryAsync();
    }
}