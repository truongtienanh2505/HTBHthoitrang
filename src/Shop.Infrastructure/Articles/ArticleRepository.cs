using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Application.Articles;

namespace Shop.Infrastructure.Articles;

public class ArticleRepository : IArticleRepository
{
    private readonly string _connStr;
    public ArticleRepository(IConfiguration config) => _connStr = config.GetConnectionString("Default")!;

    public async Task<IEnumerable<object>> GetPublishedArticlesAsync()
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT b.TieuDe, b.Slug, b.TomTat, b.AnhDaiDien, b.XuatBanLuc, c.TenChuyenMuc 
            FROM BaiViet b
            JOIN ChuyenMucBaiViet c ON b.MaChuyenMuc = c.MaChuyenMuc
            WHERE b.DaXuatBan = 1
            ORDER BY b.XuatBanLuc DESC", conn);
            
        var list = new List<object>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new {
                Title = reader["TieuDe"].ToString(),
                Slug = reader["Slug"].ToString(),
                Summary = reader["TomTat"].ToString(),
                Image = reader["AnhDaiDien"].ToString(),
                Category = reader["TenChuyenMuc"].ToString(),
                PublishedAt = reader["XuatBanLuc"]
            });
        }
        return list;
    }

    public async Task<object?> GetArticleDetailAsync(string slug)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT b.TieuDe, b.NoiDung, b.AnhDaiDien, b.XuatBanLuc, c.TenChuyenMuc 
            FROM BaiViet b
            JOIN ChuyenMucBaiViet c ON b.MaChuyenMuc = c.MaChuyenMuc
            WHERE b.Slug = @Slug AND b.DaXuatBan = 1", conn);
        cmd.Parameters.AddWithValue("@Slug", slug);
        
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new {
            Title = reader["TieuDe"].ToString(),
            Content = reader["NoiDung"].ToString(),
            Image = reader["AnhDaiDien"].ToString(),
            Category = reader["TenChuyenMuc"].ToString(),
            PublishedAt = reader["XuatBanLuc"]
        };
    }
}