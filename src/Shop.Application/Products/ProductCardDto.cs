namespace Shop.Application.Products;

public sealed record ProductCardDto(
    int MaSanPham,
    string TenSanPham,
    string Slug,
    string? Anh,
    decimal GiaThapNhat,
    decimal GiaSauGiamThapNhat
);