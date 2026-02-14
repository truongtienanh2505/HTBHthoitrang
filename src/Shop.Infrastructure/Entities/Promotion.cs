namespace Shop.Infrastructure.Entities
{
    public enum PromotionType
{
    Percentage = 1,
    FixedAmount = 2
}
    public class Promotion
{
    public int Id { get; set; }

    public string TenKhuyenMai { get; set; } = null!;

    public decimal GiaTriGiam { get; set; }

    // 1 = % , 2 = số tiền
    public int LoaiGiam { get; set; }

    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }

    public bool IsActive { get; set; }
}

}
