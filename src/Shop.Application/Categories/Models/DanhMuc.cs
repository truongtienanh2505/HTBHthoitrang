namespace Shop.Application.Categories.Models;

public class DanhMuc
{
    public int MaDanhMuc { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int? MaDanhMucCha { get; set; }
    public bool HoatDong { get; set; } = true;
    public DateTime TaoLuc { get; set; }
    public DateTime? CapNhatLuc { get; set; }

    public DanhMuc? DanhMucCha { get; set; }
    public List<DanhMuc> DanhMucCon { get; set; } = new();
}
