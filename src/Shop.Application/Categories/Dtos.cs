namespace Shop.Application.Categories;

public record DanhMucDto(int MaDanhMuc, string TenDanhMuc, string Slug, int? MaDanhMucCha, bool HoatDong);

public record CreateDanhMucRequest(string TenDanhMuc, string? Slug, int? MaDanhMucCha, bool HoatDong = true);

public record UpdateDanhMucRequest(string TenDanhMuc, string? Slug, int? MaDanhMucCha, bool HoatDong);

public class DanhMucTreeNodeDto
{
    public int MaDanhMuc { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int? MaDanhMucCha { get; set; }
    public bool HoatDong { get; set; }
    public int Level { get; set; }
    public List<DanhMucTreeNodeDto> Children { get; set; } = new();
}
