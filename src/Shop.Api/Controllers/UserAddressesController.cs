using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/user-addresses")]
    public class UserAddressesController : ControllerBase
    {
        private readonly string _connectionString;

        public UserAddressesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            int userId = 1; // Mặc định User ID = 1 để test
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            
            var query = @"
                SELECT MaDiaChi, TenLienHe, DienThoaiLienHe, DiaChi, TinhThanh, QuanHuyen, PhuongXa, MacDinh 
                FROM DiaChiNguoiDung 
                WHERE MaNguoiDung = @UserId 
                ORDER BY MacDinh DESC";
                
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            var list = new List<object>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new {
                    id = reader["MaDiaChi"],
                    contactName = reader["TenLienHe"].ToString(),
                    contactPhone = reader["DienThoaiLienHe"].ToString(),
                    addressLine = reader["DiaChi"].ToString(),
                    province = reader["TinhThanh"].ToString(),
                    district = reader["QuanHuyen"].ToString(),
                    ward = reader["PhuongXa"].ToString(),
                    isDefault = Convert.ToBoolean(reader["MacDinh"])
                });
            }
            return Ok(list);
        }
    }
}