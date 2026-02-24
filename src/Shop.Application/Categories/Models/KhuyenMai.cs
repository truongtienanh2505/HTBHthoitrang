using Shop.Application.Categories.Models;
public class KhuyenMai
{
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public int MaKhuyenMai { get; set; }
    public string TenKhuyenMai { get; set; } = null!;
    public string LoaiGiamGia { get; set; } = null!; // FIXED_AMOUNT hoặc PERCENTAGE
    public decimal GiaTriGiam { get; set; }
    public decimal? GiamToiDa { get; set; }
    public DateTime BatDau { get; set; }
    public DateTime KetThuc { get; set; }
    public bool KichHoat { get; set; }
    public int UuTien { get; set; }

    // Navigation Properties
    public virtual ICollection<DieuKienKhuyenMai> DieuKiens { get; set; } = new List<DieuKienKhuyenMai>();
    // Thêm dòng này vào class KhuyenMai để EF Core có thể thực hiện mapping WithMany
    public virtual ICollection<SanPhamKhuyenMai> SanPhamKhuyenMais { get; set; } = new List<SanPhamKhuyenMai>();

}