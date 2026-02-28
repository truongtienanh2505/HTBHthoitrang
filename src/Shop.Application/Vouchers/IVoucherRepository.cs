namespace Shop.Application.Vouchers;

public interface IVoucherRepository
{
    Task<VoucherApplyResultDto> PreviewAsync(string code, int userId, decimal orderTotal, CancellationToken ct);

    /// <summary>
    /// Consume voucher theo transaction để chống race condition (2 request xài cùng lúc).
    /// </summary>
    Task<VoucherApplyResultDto> ConsumeAsync(string code, int userId, decimal orderTotal, CancellationToken ct);
}