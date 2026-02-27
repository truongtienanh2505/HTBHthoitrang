namespace Shop.Infrastructure.Persistence;

public class ProductCardRow
{
    public int MaSanPham { get; set; }
    public string TenSanPham { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Anh { get; set; }
    public decimal GiaThapNhat { get; set; }
    public decimal GiaSauGiamThapNhat { get; set; }
}