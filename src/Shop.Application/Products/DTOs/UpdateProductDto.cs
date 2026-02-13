namespace Shop.Application.Products.Dtos;

public class UpdateProductDto
{
    public string TenSanPham { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal GiaGoc { get; set; }
    public int MaDanhMuc { get; set; }
    public bool HoatDong { get; set; }
    
}
