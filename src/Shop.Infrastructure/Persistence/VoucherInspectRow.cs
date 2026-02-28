namespace Shop.Infrastructure.Persistence;

/// <summary>
/// Keyless row dùng để đọc voucher + khuyến mãi kèm lock hints (UPDLOCK/HOLDLOCK).
/// </summary>
public sealed class VoucherInspectRow
{
    public int MaMaGiamGia { get; set; }
    public string Code { get; set; } = null!;
    public int? MaNguoiDung { get; set; }
    public int MaKhuyenMai { get; set; }
    public bool DaDung { get; set; }
    public DateTime? DungLuc { get; set; }
    public DateTime HetHanLuc { get; set; }

    public bool KichHoat { get; set; }
    public DateTime BatDau { get; set; }
    public DateTime KetThuc { get; set; }
    public string LoaiGiamGia { get; set; } = null!;
    public decimal GiaTriGiam { get; set; }
    public decimal? GiamToiDa { get; set; }
}