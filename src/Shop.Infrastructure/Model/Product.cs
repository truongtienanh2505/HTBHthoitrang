using Shop.Application.Categories.Models;
public class Product
{
    public int MaSanPham { get; set; }
    public string TenSanPham { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal GiaGoc { get; set; }
    public int MaDanhMuc { get; set; }
    public bool HoatDong { get; set; } = true;

    public virtual DanhMuc DanhMuc { get; set; } = null!;
    public virtual ICollection<ProductVariant> BienThes { get; set; } = new List<ProductVariant>();
}
