using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly string _connStr;
    public ArticlesController(IConfiguration config) => _connStr = config.GetConnectionString("Default");

    // 1. Lấy danh sách Chuyên mục & Bài viết (Khớp với listBlog của index.js)
    [HttpGet]
    public async Task<IActionResult> GetAllArticles([FromQuery] string? cat)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        // Lấy danh sách Chuyên mục
        var cats = new List<object>();
        using (var cmdCat = new SqlCommand("SELECT MaChuyenMuc, TenChuyenMuc, Slug FROM ChuyenMucBaiViet", conn))
        using (var rCat = await cmdCat.ExecuteReaderAsync())
        {
            while (await rCat.ReadAsync())
                cats.Add(new { MaChuyenMuc = rCat["MaChuyenMuc"], TenChuyenMuc = rCat["TenChuyenMuc"].ToString(), Slug = rCat["Slug"].ToString() });
        }

        // Lấy danh sách Bài viết (Có lọc theo chuyên mục nếu có truyền cat)
        var posts = new List<object>();
        var query = "SELECT MaBaiViet, TieuDe, TomTat, AnhDaiDien, MaChuyenMuc, Slug FROM BaiViet WHERE DaXuatBan = 1";
        if (!string.IsNullOrEmpty(cat)) {
            query += " AND MaChuyenMuc = (SELECT TOP 1 MaChuyenMuc FROM ChuyenMucBaiViet WHERE Slug = @CatSlug)";
        }
        query += " ORDER BY TaoLuc DESC";

        using (var cmdPost = new SqlCommand(query, conn))
        {
            if (!string.IsNullOrEmpty(cat)) cmdPost.Parameters.AddWithValue("@CatSlug", cat);
            using var rPost = await cmdPost.ExecuteReaderAsync();
            while (await rPost.ReadAsync())
            {
                posts.Add(new {
                    MaChuyenMuc = rPost["MaChuyenMuc"],
                    TieuDe = rPost["TieuDe"].ToString(),
                    TomTat = rPost["TomTat"].ToString(),
                    AnhDaiDien = rPost["AnhDaiDien"]?.ToString() ?? "",
                    Slug = rPost["Slug"].ToString()
                });
            }
        }

        return Ok(new { categories = cats, posts = posts });
    }

    // 2. Lấy chi tiết bài viết qua Slug (Khớp với getPostBySlug của post.js)
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetArticleBySlug(string slug)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        var cmd = new SqlCommand(@"
            SELECT b.TieuDe, b.XuatBanLuc, b.NoiDung, c.TenChuyenMuc 
            FROM BaiViet b
            LEFT JOIN ChuyenMucBaiViet c ON b.MaChuyenMuc = c.MaChuyenMuc
            WHERE b.Slug = @Slug AND b.DaXuatBan = 1", conn);
        cmd.Parameters.AddWithValue("@Slug", slug);
        
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return NotFound(new { message = "Không tìm thấy bài viết" });

        return Ok(new {
            post = new {
                TieuDe = reader["TieuDe"].ToString(),
                XuatBanLuc = reader["XuatBanLuc"]?.ToString(),
                NoiDung = reader["NoiDung"].ToString()
            },
            category = new {
                TenChuyenMuc = reader["TenChuyenMuc"]?.ToString() ?? ""
            }
        });
    }
}