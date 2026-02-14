public class UpdatePromotionDto
{
    public string TenKhuyenMai { get; set; } = null!;
    public decimal PhanTramGiam { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public bool TrangThai { get; set; }
}
