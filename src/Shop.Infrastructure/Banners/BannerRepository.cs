using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Application.Banners;

namespace Shop.Infrastructure.Banners;
public class BannerRepository : IBannerRepository
{
    private readonly string _connStr;
    public BannerRepository(IConfiguration config) => _connStr = config.GetConnectionString("Default")!;

    public async Task<IEnumerable<object>> GetActiveBannersAsync()
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        
        // Đã sửa lại đúng tên cột KichHoat và bỏ cột MoTa
        var cmd = new SqlCommand("SELECT TieuDe, UrlAnh, UrlLienKet FROM BannerTrangChu WHERE KichHoat = 1 ORDER BY ThuTu ASC", conn);
        var list = new List<object>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new {
                TieuDe = reader["TieuDe"].ToString(),
                UrlAnh = reader["UrlAnh"].ToString(),
                UrlLienKet = reader["UrlLienKet"].ToString()
            });
        }
        return list;
    }
}