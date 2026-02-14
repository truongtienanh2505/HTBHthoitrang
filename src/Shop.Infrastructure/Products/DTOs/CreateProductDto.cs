namespace Shop.Application.Products.DTOs;

public class CreateProductDto
{
    public string TenSanPham { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal GiaGoc { get; set; }
    public int MaDanhMuc { get; set; }
}
