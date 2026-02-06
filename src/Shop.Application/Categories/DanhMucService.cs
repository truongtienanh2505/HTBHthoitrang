using System.Globalization;
using System.Text;
using Shop.Application.Categories.Models;


namespace Shop.Application.Categories;

public class DanhMucService
{
    private readonly IDanhMucRepository _repo;

    public DanhMucService(IDanhMucRepository repo) => _repo = repo;

    public async Task<List<DanhMucDto>> GetAllAsync(bool includeInactive, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(includeInactive, ct);
        return list.Select(x => new DanhMucDto(x.MaDanhMuc, x.TenDanhMuc, x.Slug, x.MaDanhMucCha, x.HoatDong)).ToList();
    }

    public async Task<DanhMucDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var x = await _repo.GetByIdAsync(id, ct);
        return x is null ? null : new DanhMucDto(x.MaDanhMuc, x.TenDanhMuc, x.Slug, x.MaDanhMucCha, x.HoatDong);
    }

    public async Task<int> CreateAsync(CreateDanhMucRequest req, CancellationToken ct)
    {
        var slug = string.IsNullOrWhiteSpace(req.Slug) ? Slugify(req.TenDanhMuc) : Slugify(req.Slug);

        var entity = new DanhMuc
        {
            TenDanhMuc = req.TenDanhMuc.Trim(),
            Slug = slug,
            MaDanhMucCha = req.MaDanhMucCha,
            HoatDong = req.HoatDong,
            TaoLuc = DateTime.UtcNow
        };

        return await _repo.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDanhMucRequest req, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (req.MaDanhMucCha == id) throw new InvalidOperationException("MaDanhMucCha không được trỏ tới chính nó.");

        entity.TenDanhMuc = req.TenDanhMuc.Trim();
        entity.Slug = string.IsNullOrWhiteSpace(req.Slug) ? Slugify(req.TenDanhMuc) : Slugify(req.Slug);
        entity.MaDanhMucCha = req.MaDanhMucCha;
        entity.HoatDong = req.HoatDong;
        entity.CapNhatLuc = DateTime.UtcNow;

        await _repo.UpdateAsync(entity, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return false;

        await _repo.DeleteAsync(entity, ct);
        return true;
    }

    public async Task<List<DanhMucTreeNodeDto>> GetTreeAsync(CancellationToken ct)
    {
        var rows = await _repo.GetTreeRowsAsync(ct);

        var dict = rows.ToDictionary(
            r => r.MaDanhMuc,
            r => new DanhMucTreeNodeDto
            {
                MaDanhMuc = r.MaDanhMuc,
                TenDanhMuc = r.TenDanhMuc,
                Slug = r.Slug,
                MaDanhMucCha = r.MaDanhMucCha,
                HoatDong = r.HoatDong,
                Level = r.Level
            });

        var roots = new List<DanhMucTreeNodeDto>();

        foreach (var node in dict.Values)
        {
            if (node.MaDanhMucCha is null)
            {
                roots.Add(node);
                continue;
            }

            if (dict.TryGetValue(node.MaDanhMucCha.Value, out var parent))
                parent.Children.Add(node);
            else
                roots.Add(node); // fallback nếu data bể
        }

        return roots;
    }

    private static string Slugify(string input)
    {
        var s = input.Trim().ToLowerInvariant();

        var normalized = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
        }

        s = sb.ToString().Normalize(NormalizationForm.FormC);
        s = s.Replace('đ', 'd').Replace('Đ', 'D');

        var outSb = new StringBuilder();
        bool dash = false;
        foreach (var ch in s)
        {
            if (char.IsLetterOrDigit(ch))
            {
                outSb.Append(ch);
                dash = false;
            }
            else
            {
                if (!dash)
                {
                    outSb.Append('-');
                    dash = true;
                }
            }
        }

        return outSb.ToString().Trim('-');
    }
}
