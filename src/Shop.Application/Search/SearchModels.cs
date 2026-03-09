namespace Shop.Application.Search;

// Dữ liệu khách hàng gửi lên (Đầu vào)
public class SearchRequest
{
    public string? Keyword { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MaMauSac { get; set; }
    public int? MaKichCo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12; // Mặc định lấy 12 sản phẩm/trang
}

// Dữ liệu trả về cho Frontend (Đầu ra)
public class SearchResult
{
    public int MaSanPham { get; set; }
    public string TenSanPham { get; set; } = string.Empty;
    public decimal GiaGoc { get; set; }
    public string? AnhDaiDien { get; set; }
}
