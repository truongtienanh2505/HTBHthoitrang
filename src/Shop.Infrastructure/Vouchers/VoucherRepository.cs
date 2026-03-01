using Microsoft.EntityFrameworkCore;
using Shop.Application.Vouchers;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Vouchers;

public sealed class VoucherRepository : IVoucherRepository
{
    private readonly ShopDbContext _db;
    public VoucherRepository(ShopDbContext db) => _db = db;

    public Task<VoucherApplyResultDto> PreviewAsync(string code, int userId, decimal orderTotal, CancellationToken ct)
        => ApplyInternalAsync(code, userId, orderTotal, consume: false, ct);

    public Task<VoucherApplyResultDto> ConsumeAsync(string code, int userId, decimal orderTotal, CancellationToken ct)
        => ApplyInternalAsync(code, userId, orderTotal, consume: true, ct);

    private async Task<VoucherApplyResultDto> ApplyInternalAsync(string code, int userId, decimal orderTotal, bool consume, CancellationToken ct)
    {
        code = (code ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(code) || orderTotal <= 0)
        {
            return new VoucherApplyResultDto(
                VoucherApplyStatus.Invalid,
                code,
                0m,
                null,
                null,
                "Code rỗng hoặc OrderTotal không hợp lệ."
            );
        }

        // Preview không cần transaction.
        if (!consume)
        {
            var row = await ReadVoucherAsync(code, lockForUpdate: false, ct);
            return Evaluate(row, code, userId, orderTotal, consume: false);
        }

        // Consume cần transaction + lock hints để chống race.
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var lockedRow = await ReadVoucherAsync(code, lockForUpdate: true, ct);
        var evaluated = Evaluate(lockedRow, code, userId, orderTotal, consume: true);

        if (evaluated.Status != VoucherApplyStatus.Ok)
        {
            await tx.RollbackAsync(ct);
            return evaluated;
        }

        // Mark used (atomic inside same transaction)
        var now = DateTime.UtcNow;

        var updateSql = """
        UPDATE dbo.MaGiamGia
        SET SoLanDaDung = SoLanDaDung + 1,
            DungLuc = {2},
            DaDung = CASE WHEN SoLanDaDung + 1 >= SoLanToiDa THEN 1 ELSE 0 END
        WHERE MaMaGiamGia = {0}
            AND SoLanDaDung < SoLanToiDa
            AND HetHanLuc > {2}
            AND (MaNguoiDung IS NULL OR MaNguoiDung = {1});
        """;        

        var affected = await _db.Database.ExecuteSqlRawAsync(
            updateSql,
            new object?[] { evaluated.MaMaGiamGia!.Value, userId, now },
            ct);

        if (affected != 1)
        {
            await tx.RollbackAsync(ct);
            return new VoucherApplyResultDto(
                VoucherApplyStatus.AlreadyUsed,
                code,
                0m,
                null,
                null,
                "Voucher vừa được dùng bởi request khác (race condition)."
            );
        }

        await tx.CommitAsync(ct);

// lockedRow là trạng thái trước UPDATE, nên sau consume sẽ là +1
        var usedAfter = lockedRow!.SoLanDaDung + 1;
        var maxUses = lockedRow.SoLanToiDa;
        var remaining = Math.Max(0, maxUses - usedAfter);

        return evaluated with
        {
            SoLanDaDung = usedAfter,
            SoLanToiDa = maxUses,
            SoLanConLai = remaining
        };
        
    }

    private Task<VoucherInspectRow?> ReadVoucherAsync(string code, bool lockForUpdate, CancellationToken ct)
{
    const string SqlUnlocked = """
    SELECT TOP (1)
        mg.MaMaGiamGia,
        mg.Code,
        mg.MaNguoiDung,
        mg.MaKhuyenMai,
        mg.SoLanToiDa,
        mg.SoLanDaDung,
        mg.DaDung,
        mg.DungLuc,
        mg.HetHanLuc,
        km.KichHoat,
        km.BatDau,
        km.KetThuc,
        km.LoaiGiamGia,
        km.GiaTriGiam,
        km.GiamToiDa
    FROM dbo.MaGiamGia mg
    JOIN dbo.KhuyenMai km ON km.MaKhuyenMai = mg.MaKhuyenMai
    WHERE mg.Code = {0}
    """;

    const string SqlLocked = """
    SELECT TOP (1)
        mg.MaMaGiamGia,
        mg.Code,
        mg.MaNguoiDung,
        mg.MaKhuyenMai,
        mg.SoLanToiDa,
        mg.SoLanDaDung,
        mg.DaDung,
        mg.DungLuc,
        mg.HetHanLuc,
        km.KichHoat,
        km.BatDau,
        km.KetThuc,
        km.LoaiGiamGia,
        km.GiaTriGiam,
        km.GiamToiDa
    FROM dbo.MaGiamGia mg WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
    JOIN dbo.KhuyenMai km ON km.MaKhuyenMai = mg.MaKhuyenMai
    WHERE mg.Code = {0}
    """;

    var sql = lockForUpdate ? SqlLocked : SqlUnlocked;

    return _db.VoucherInspectRows
        .FromSqlRaw(sql, code)
        .AsNoTracking()
        .SingleOrDefaultAsync(ct);
}

    private static VoucherApplyResultDto Evaluate(VoucherInspectRow? row, string code, int userId, decimal orderTotal, bool consume)
    {
        if (row is null)
            return new(VoucherApplyStatus.NotFound, code, 0m, null, null, "Không tìm thấy voucher.");

        var now = DateTime.UtcNow;

        if (row.DaDung || row.SoLanDaDung >= row.SoLanToiDa)
            return new(VoucherApplyStatus.AlreadyUsed, code, 0m, row.MaMaGiamGia, row.MaKhuyenMai, "Voucher đã hết lượt sử dụng.");

        if (row.HetHanLuc <= now)
            return new(VoucherApplyStatus.Expired, code, 0m, row.MaMaGiamGia, row.MaKhuyenMai, "Voucher đã hết hạn.");

        if (row.MaNguoiDung is not null && row.MaNguoiDung.Value != userId)
            return new(VoucherApplyStatus.NotOwned, code, 0m, row.MaMaGiamGia, row.MaKhuyenMai, "Voucher này thuộc user khác.");

        var promoActive = row.KichHoat && row.BatDau <= now && row.KetThuc > now;
        if (!promoActive)
            return new(VoucherApplyStatus.PromotionInactive, code, 0m, row.MaMaGiamGia, row.MaKhuyenMai, "Khuyến mãi đang tắt hoặc ngoài thời gian hiệu lực.");

        var discount = CalcDiscount(orderTotal, row.LoaiGiamGia, row.GiaTriGiam, row.GiamToiDa);
        if (discount <= 0)
            return new(VoucherApplyStatus.Invalid, code, 0m, row.MaMaGiamGia, row.MaKhuyenMai, "Voucher không áp dụng được cho đơn này.");

        return new(VoucherApplyStatus.Ok, code, discount, row.MaMaGiamGia, row.MaKhuyenMai, consume ? "Consume voucher thành công." : "Preview voucher hợp lệ.");
    }

    private static decimal CalcDiscount(decimal orderTotal, string loaiGiamGia, decimal giaTriGiam, decimal? giamToiDa)
    {
        orderTotal = Math.Max(0m, orderTotal);
        if (orderTotal <= 0) return 0m;

        decimal discount = loaiGiamGia switch
        {
            "PERCENTAGE" => orderTotal * giaTriGiam / 100m,
            "FIXED_AMOUNT" => giaTriGiam,
            _ => 0m
        };

        if (giamToiDa is not null)
            discount = Math.Min(discount, giamToiDa.Value);

        discount = Math.Min(discount, orderTotal);
        return Math.Max(0m, decimal.Round(discount, 2));
    }
}