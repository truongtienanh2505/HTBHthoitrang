namespace Shop.Application.Categories.Models;

public class DanhMucTreeRow
{
    public int MaDanhMuc { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int? MaDanhMucCha { get; set; }
    public bool HoatDong { get; set; }
    public int Level { get; set; }
    public string SortPath { get; set; } = null!;
}

