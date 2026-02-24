namespace Shop.Application.Categories.Dtos
{
   public class PromotionRequest
{
    public string TenKhuyenMai { get; set; } = null!;
    public string LoaiGiamGia { get; set; } = null!;
    public decimal GiaTriGiam { get; set; }
    public bool KichHoat { get; set; }
    // Bổ sung 2 dòng này để nhận dữ liệu từ Swagger
    public DateTime NgayBatDau { get; set; } 
    public DateTime NgayKetThuc { get; set; }
    public List<int> DanhSachMaSanPham { get; set; } = new();
}
}