using Microsoft.EntityFrameworkCore;
using Shop.Application.Orders.Models;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Orders;

public class OrderService
{
    private readonly ShopDbContext _context;

    public OrderService(ShopDbContext context)
    {
        _context = context;
    }

    // 1. API VIEW: Xem danh sách đơn hàng của User
    public async Task<List<DonHang>> GetMyOrdersAsync(int maNguoiDung)
    {
        return await _context.DonHangs
            .AsNoTracking()
            .Where(x => x.MaNguoiDung == maNguoiDung)
            .OrderByDescending(x => x.NgayDat)
            .ToListAsync();
    }

    // 2. STATE MACHINE (SLIDE 110): Kỹ thuật chặn chuyển đổi trạng thái sai
    private bool CanChangeState(TrangThaiDonHang current, TrangThaiDonHang next)
    {
        return current switch
        {
            TrangThaiDonHang.ChoXacNhan => next == TrangThaiDonHang.DaXacNhan || next == TrangThaiDonHang.DaHuy,
            TrangThaiDonHang.DaXacNhan => next == TrangThaiDonHang.DangGiaoHang || next == TrangThaiDonHang.DaHuy,
            TrangThaiDonHang.DangGiaoHang => next == TrangThaiDonHang.DaGiaoThanhCong || next == TrangThaiDonHang.DaHuy,
            TrangThaiDonHang.DaGiaoThanhCong => false, // Đã giao xong thì khóa vĩnh viễn
            TrangThaiDonHang.DaHuy => false,           // Đã hủy thì khóa vĩnh viễn
            _ => false
        };
    }

    // 3. Cập nhật trạng thái + Tự động lưu lịch sử
    public async Task<bool> ChangeOrderStatusAsync(int maDonHang, TrangThaiDonHang trangThaiMoi, string ghiChu)
    {
        var order = await _context.DonHangs.FindAsync(maDonHang);
        if (order == null) throw new Exception("Không tìm thấy đơn hàng này.");

        // Gọi State Machine kiểm tra chặn lỗi
        if (!CanChangeState(order.TrangThai, trangThaiMoi))
        {
            throw new Exception($"Vi phạm State Machine: Không thể chuyển nhảy cóc từ '{order.TrangThai}' sang '{trangThaiMoi}'.");
        }

        // Lưu vết lịch sử
        var history = new LichSuDonHang
        {
            MaDonHang = maDonHang,
            TrangThaiCu = order.TrangThai,
            TrangThaiMoi = trangThaiMoi,
            GhiChu = ghiChu,
            NgayThayDoi = DateTime.Now
        };
        _context.LichSuDonHangs.Add(history);

        // Cập nhật đơn
        order.TrangThai = trangThaiMoi;
        await _context.SaveChangesAsync();
        return true;
    }

    // 4. Lấy lịch sử tracking
    public async Task<List<LichSuDonHang>> GetOrderHistoryAsync(int maDonHang)
    {
        return await _context.LichSuDonHangs
            .AsNoTracking()
            .Where(x => x.MaDonHang == maDonHang)
            .OrderByDescending(x => x.NgayThayDoi)
            .ToListAsync();
    }
}