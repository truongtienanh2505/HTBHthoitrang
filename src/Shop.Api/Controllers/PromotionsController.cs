using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products;
using Shop.Application.Categories.Dtos; // Thêm dòng này
using Shop.Application.Categories.Models;
namespace Shop.Api.Controllers;
using Shop.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore; // Để hết lỗi 
[Route("api/[controller]")]
[ApiController]
public class PromotionsController : ControllerBase
{
    private readonly ShopDbContext _context;
    public PromotionsController(ShopDbContext context) => _context = context;

    // 1. Lấy danh sách khuyến mãi
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.KhuyenMais
            .Include(x => x.SanPhamKhuyenMais)
            .ToListAsync();
        return Ok(data);
    }
    [HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var promo = await _context.KhuyenMais
        .Include(x => x.SanPhamKhuyenMais)
            .ThenInclude(sp => sp.SanPham) // Sử dụng tên property Sản phẩm trong class SanPhamKhuyenMai
        .FirstOrDefaultAsync(x => x.MaKhuyenMai == id);

    if (promo == null) return NotFound("Không tìm thấy khuyến mãi");
    return Ok(promo);
}
    // 2. Tạo mới Khuyến mãi và gán vào Sản phẩm
    [HttpPost]
public async Task<IActionResult> Create(PromotionRequest request)
{
    var newPromo = new KhuyenMai
    {
        TenKhuyenMai = request.TenKhuyenMai,
        LoaiGiamGia = request.LoaiGiamGia,
        GiaTriGiam = request.GiaTriGiam,
        KichHoat = request.KichHoat,
        // Gán dữ liệu ngày tháng tại đây
        NgayBatDau = request.NgayBatDau, 
        NgayKetThuc = request.NgayKetThuc,
        
        SanPhamKhuyenMais = request.DanhSachMaSanPham.Select(id => new SanPhamKhuyenMai
        {
            MaSanPham = id
        }).ToList()
    };

    _context.KhuyenMais.Add(newPromo);
    await _context.SaveChangesAsync(); // Lỗi 500 xảy ra tại dòng này nếu ngày sai
    return Ok(newPromo);
}
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, PromotionRequest request)
{
    var promo = await _context.KhuyenMais.FindAsync(id);
    if (promo == null) return NotFound();

    // Cập nhật thông tin mới từ request
    promo.TenKhuyenMai = request.TenKhuyenMai;
    promo.GiaTriGiam = request.GiaTriGiam;
    promo.NgayBatDau = request.NgayBatDau;
    promo.NgayKetThuc = request.NgayKetThuc;
    promo.KichHoat = request.KichHoat;

    await _context.SaveChangesAsync();
    return NoContent();
}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var promo = await _context.KhuyenMais.FindAsync(id);
    if (promo == null) return NotFound();

    _context.KhuyenMais.Remove(promo);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Đã xóa khuyến mãi thành công" });
}
}