using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Application.Products;

namespace Shop.Infrastructure.Products;

public class ProductRepository : IProductRepository
{
    private readonly string _connStr;
    public ProductRepository(IConfiguration config) => _connStr = config.GetConnectionString("Default")!;

    public async Task<(int Total, IEnumerable<object> Items)> GetProductsAsync(string? search, string? cat, string? sort, decimal? minPrice, decimal? maxPrice, int page, int pageSize)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();

        // Xây dựng điều kiện lọc động (Dynamic WHERE)
        string whereClause = "WHERE HoatDong = 1"; // Chỉ lấy sp đang bán
        if (!string.IsNullOrEmpty(search)) whereClause += " AND TenSanPham LIKE '%' + @Search + '%'";
        if (minPrice.HasValue) whereClause += " AND GiaGoc >= @MinPrice";
        if (maxPrice.HasValue) whereClause += " AND GiaGoc <= @MaxPrice";
        
        // Đoạn này nếu DB của bạn có bảng DanhMuc, bạn có thể JOIN. Tạm thời lọc theo tên/slug danh mục.
        if (!string.IsNullOrEmpty(cat)) whereClause += " AND MaDanhMuc IN (SELECT MaDanhMuc FROM DanhMuc WHERE Slug = @Cat)";

        // Xây dựng câu lệnh Sắp xếp động
        string orderBy = sort switch
        {
            "new" => "ORDER BY MaSanPham DESC",
            "price_asc" => "ORDER BY GiaGoc ASC",
            "price_desc" => "ORDER BY GiaGoc DESC",
            _ => "ORDER BY MaSanPham DESC" // Mặc định
        };

        // 1. Đếm tổng số sản phẩm (để Frontend làm phân trang)
        var cmdCount = new SqlCommand($"SELECT COUNT(*) FROM SanPham {whereClause}", conn);
        if (!string.IsNullOrEmpty(search)) cmdCount.Parameters.AddWithValue("@Search", search);
        if (minPrice.HasValue) cmdCount.Parameters.AddWithValue("@MinPrice", minPrice.Value);
        if (maxPrice.HasValue) cmdCount.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);
        if (!string.IsNullOrEmpty(cat)) cmdCount.Parameters.AddWithValue("@Cat", cat);
        
        int total = (int)await cmdCount.ExecuteScalarAsync();

        // 2. Lấy dữ liệu theo trang (OFFSET FETCH)
        int offset = (page - 1) * pageSize;
        var cmdList = new SqlCommand($@"
            SELECT MaSanPham, TenSanPham, Slug, GiaGoc, AnhDaiDien 
            FROM SanPham 
            {whereClause} 
            {orderBy} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn);

        // Copy lại các tham số
        foreach (SqlParameter p in cmdCount.Parameters) cmdList.Parameters.AddWithValue(p.ParameterName, p.Value);
        cmdList.Parameters.AddWithValue("@Offset", offset);
        cmdList.Parameters.AddWithValue("@PageSize", pageSize);

        var items = new List<object>();
        using var reader = await cmdList.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new {
                MaSanPham = reader["MaSanPham"],
                TenSanPham = reader["TenSanPham"].ToString(),
                Slug = reader["Slug"].ToString(),
                GiaGoc = reader["GiaGoc"],
                AnhDaiDien = reader["AnhDaiDien"].ToString() 
            });
        }

        return (total, items);
    }

    public async Task<object?> GetProductDetailAsync(int id)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();

        // 1. Lấy thông tin gốc sản phẩm
        var cmdProd = new SqlCommand("SELECT * FROM SanPham WHERE MaSanPham = @Id", conn);
        cmdProd.Parameters.AddWithValue("@Id", id);
        using var reader = await cmdProd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var product = new {
            MaSanPham = reader["MaSanPham"],
            TenSanPham = reader["TenSanPham"].ToString(),
            Slug = reader["Slug"].ToString(),
            MoTa = reader["MoTa"].ToString(),
            GiaGoc = reader["GiaGoc"]
        };
        await reader.CloseAsync();

        // 2. Lấy danh sách biến thể (Màu, Size)
        var cmdVars = new SqlCommand("SELECT * FROM BienTheSanPham WHERE MaSanPham = @Id", conn);
        cmdVars.Parameters.AddWithValue("@Id", id);
        var variants = new List<object>();
        using var readerVars = await cmdVars.ExecuteReaderAsync();
        while (await readerVars.ReadAsync())
        {
            variants.Add(new {
                MaMauSac = readerVars["MaMauSac"],
                MaKichCo = readerVars["MaKichCo"],
                SKU = readerVars["SKU"].ToString(),
                SoLuongTon = readerVars["SoLuongTon"],
                DieuChinhGia = readerVars["DieuChinhGia"]
            });
        }

        // Trả về format chuẩn { product, variants, images }
        return new { product = product, variants = variants, images = new List<object>() };
    }
}