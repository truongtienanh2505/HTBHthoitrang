namespace Shop.Application.Vouchers;

public enum VoucherApplyStatus
{
    Ok = 0,
    NotFound = 1,
    AlreadyUsed = 2,
    Expired = 3,
    NotOwned = 4,
    PromotionInactive = 5,
    Invalid = 6
}

public sealed record VoucherApplyRequest(string Code, decimal OrderTotal);

public sealed record VoucherApplyResultDto(
    VoucherApplyStatus Status,
    string Code,
    decimal TienGiam,
    int? MaMaGiamGia,
    int? MaKhuyenMai,
    string Message
)
{
    public int? SoLanDaDung { get; init; }
    public int? SoLanToiDa { get; init; }
    public int? SoLanConLai { get; init; }
}