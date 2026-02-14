using Shop.Application.Categories.Models;

namespace Shop.Application.Categories;

public interface IDanhMucRepository
{
    Task<List<DanhMuc>> GetAllAsync(bool includeInactive, CancellationToken ct);
    Task<DanhMuc?> GetByIdAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(DanhMuc entity, CancellationToken ct);
    Task UpdateAsync(DanhMuc entity, CancellationToken ct);
    Task DeleteAsync(DanhMuc entity, CancellationToken ct);

    Task<List<DanhMucTreeRow>> GetTreeRowsAsync(CancellationToken ct);
}
