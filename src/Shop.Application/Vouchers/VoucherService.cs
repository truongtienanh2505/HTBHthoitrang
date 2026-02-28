namespace Shop.Application.Vouchers;

public sealed class VoucherService
{
    private readonly IVoucherRepository _repo;
    public VoucherService(IVoucherRepository repo) => _repo = repo;

    public Task<VoucherApplyResultDto> PreviewAsync(int userId, VoucherApplyRequest req, CancellationToken ct)
        => _repo.PreviewAsync(req.Code, userId, req.OrderTotal, ct);

    public Task<VoucherApplyResultDto> ConsumeAsync(int userId, VoucherApplyRequest req, CancellationToken ct)
        => _repo.ConsumeAsync(req.Code, userId, req.OrderTotal, ct);
}