using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // Hoặc dùng System.Data.SqlClient nếu báo lỗi
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly string _connectionString;

        // Tự động đọc chuỗi kết nối "Default" từ appsettings.json
        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // 1. Lấy thông tin cơ bản Sản phẩm
            var productQuery = @"
                SELECT sp.*, dm.TenDanhMuc 
                FROM SanPham sp
                LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
                WHERE sp.MaSanPham = @Id";
            
            using var cmd1 = new SqlCommand(productQuery, conn);
            cmd1.Parameters.AddWithValue("@Id", id);
            using var reader1 = await cmd1.ExecuteReaderAsync();
            
            if (!await reader1.ReadAsync()) return NotFound(new { message = "Không tìm thấy sản phẩm" });

            var product = new {
                MaSanPham = reader1["MaSanPham"],
                TenSanPham = reader1["TenSanPham"].ToString(),
                Slug = reader1["Slug"].ToString(),
                GiaGoc = reader1["GiaGoc"],
                AnhDaiDien = reader1["AnhDaiDien"].ToString(),
                MoTa = reader1["MoTa"].ToString(),
                TenDanhMuc = reader1["TenDanhMuc"].ToString()
            };
            await reader1.CloseAsync();

            // 2. Lấy Biến thể (Màu/Size)
            var variantQuery = @"
                SELECT bt.*, m.TenMau, m.MaHex, k.TenKichCo 
                FROM BienTheSanPham bt
                JOIN MauSac m ON bt.MaMauSac = m.MaMauSac
                JOIN KichCo k ON bt.MaKichCo = k.MaKichCo
                WHERE bt.MaSanPham = @Id AND bt.HoatDong = 1";
            
            var variants = new List<object>();
            using var cmd2 = new SqlCommand(variantQuery, conn);
            cmd2.Parameters.AddWithValue("@Id", id);
            using var reader2 = await cmd2.ExecuteReaderAsync();
            
            while (await reader2.ReadAsync())
            {
                variants.Add(new {
                    MaMauSac = reader2["MaMauSac"],
                    MaKichCo = reader2["MaKichCo"],
                    TenMau = reader2["TenMau"].ToString(),
                    MaHex = reader2["MaHex"].ToString(),
                    TenKichCo = reader2["TenKichCo"].ToString(),
                    DieuChinhGia = reader2["DieuChinhGia"],
                    SoLuongTon = reader2["SoLuongTon"]
                });
            }
            await reader2.CloseAsync();

            return Ok(new {
                product = product,
                variants = variants,
                images = new[] { new { UrlAnh = product.AnhDaiDien } }
            });
        }
    }
}