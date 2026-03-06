using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Application.UserAddresses;

namespace Shop.Infrastructure.UserAddresses;

public class UserAddressRepository : IUserAddressRepository
{
    private readonly string _connectionString;

    public UserAddressRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")!;
    }

    public async Task<IEnumerable<object>> GetAllAsync(int userId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        var query = @"
            SELECT MaDiaChi, TenLienHe, DienThoaiLienHe, DiaChi, TinhThanh, QuanHuyen, PhuongXa, MacDinh 
            FROM DiaChiNguoiDung 
            WHERE MaNguoiDung = @UserId 
            ORDER BY MacDinh DESC, TaoLuc DESC";
            
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        
        var list = new List<object>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new {
                Id = reader["MaDiaChi"],
                Name = reader["TenLienHe"].ToString(),
                Phone = reader["DienThoaiLienHe"].ToString(),
                Address = reader["DiaChi"].ToString(),
                Province = reader["TinhThanh"].ToString(),
                District = reader["QuanHuyen"].ToString(),
                Ward = reader["PhuongXa"].ToString(),
                IsDefault = Convert.ToBoolean(reader["MacDinh"])
            });
        }
        return list;
    }

    public async Task CreateAsync(int userId, AddressDto dto)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        using var trans = conn.BeginTransaction();

        try
        {
            if (dto.IsDefault)
            {
                var cmdUpdateOld = new SqlCommand("UPDATE DiaChiNguoiDung SET MacDinh = 0 WHERE MaNguoiDung = @UserId", conn, trans);
                cmdUpdateOld.Parameters.AddWithValue("@UserId", userId);
                await cmdUpdateOld.ExecuteNonQueryAsync();
            }

            var cmdInsert = new SqlCommand(@"
                INSERT INTO DiaChiNguoiDung (MaNguoiDung, TenLienHe, DienThoaiLienHe, DiaChi, TinhThanh, QuanHuyen, PhuongXa, MacDinh, TaoLuc)
                VALUES (@UserId, @Name, @Phone, @Address, @Province, @District, @Ward, @IsDefault, SYSUTCDATETIME())", conn, trans);
            
            cmdInsert.Parameters.AddWithValue("@UserId", userId);
            cmdInsert.Parameters.AddWithValue("@Name", dto.Name);
            cmdInsert.Parameters.AddWithValue("@Phone", dto.Phone);
            cmdInsert.Parameters.AddWithValue("@Address", dto.Address);
            cmdInsert.Parameters.AddWithValue("@Province", dto.Province);
            cmdInsert.Parameters.AddWithValue("@District", dto.District);
            cmdInsert.Parameters.AddWithValue("@Ward", dto.Ward);
            cmdInsert.Parameters.AddWithValue("@IsDefault", dto.IsDefault);

            await cmdInsert.ExecuteNonQueryAsync();
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, int userId, AddressDto dto)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        using var trans = conn.BeginTransaction();

        try
        {
            if (dto.IsDefault)
            {
                var cmdUpdateOld = new SqlCommand("UPDATE DiaChiNguoiDung SET MacDinh = 0 WHERE MaNguoiDung = @UserId", conn, trans);
                cmdUpdateOld.Parameters.AddWithValue("@UserId", userId);
                await cmdUpdateOld.ExecuteNonQueryAsync();
            }

            var cmdUpdate = new SqlCommand(@"
                UPDATE DiaChiNguoiDung 
                SET TenLienHe = @Name, DienThoaiLienHe = @Phone, DiaChi = @Address, 
                    TinhThanh = @Province, QuanHuyen = @District, PhuongXa = @Ward, 
                    MacDinh = @IsDefault, CapNhatLuc = SYSUTCDATETIME()
                WHERE MaDiaChi = @Id AND MaNguoiDung = @UserId", conn, trans);
            
            // Tham số tương tự Insert
            cmdUpdate.Parameters.AddWithValue("@Id", id);
            cmdUpdate.Parameters.AddWithValue("@UserId", userId);
            cmdUpdate.Parameters.AddWithValue("@Name", dto.Name);
            cmdUpdate.Parameters.AddWithValue("@Phone", dto.Phone);
            cmdUpdate.Parameters.AddWithValue("@Address", dto.Address);
            cmdUpdate.Parameters.AddWithValue("@Province", dto.Province);
            cmdUpdate.Parameters.AddWithValue("@District", dto.District);
            cmdUpdate.Parameters.AddWithValue("@Ward", dto.Ward);
            cmdUpdate.Parameters.AddWithValue("@IsDefault", dto.IsDefault);

            int rows = await cmdUpdate.ExecuteNonQueryAsync();
            trans.Commit();
            return rows > 0;
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new SqlCommand("DELETE FROM DiaChiNguoiDung WHERE MaDiaChi = @Id AND MaNguoiDung = @UserId", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserId", userId);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}