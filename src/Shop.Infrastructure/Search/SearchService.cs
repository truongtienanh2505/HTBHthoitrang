using Microsoft.EntityFrameworkCore;
using Shop.Application.Search;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Search;

public class SearchService
{
    private readonly ShopDbContext _context;

    public SearchService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<object> SearchProductsAsync(SearchRequest req)
    {
        var page = req.Page <= 0 ? 1 : req.Page;
        var pageSize = req.PageSize <= 0 ? 12 : req.PageSize;

        var query = _context.SanPhams
            .AsNoTracking()
            .Where(sp => sp.HoatDong);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var keyword = req.Keyword.Trim();
            var pattern = $"%{keyword}%";

            query = query.Where(sp =>
                EF.Functions.Like(sp.TenSanPham, pattern) ||
                (sp.MoTa != null && EF.Functions.Like(sp.MoTa, pattern)));
        }

        if (req.MinPrice.HasValue)
            query = query.Where(sp => sp.GiaGoc >= req.MinPrice.Value);

        if (req.MaxPrice.HasValue)
            query = query.Where(sp => sp.GiaGoc <= req.MaxPrice.Value);

        var totalItems = await query.CountAsync();

        var products = await query
            .Select(sp => new SearchResult
            {
                MaSanPham = sp.MaSanPham,
                TenSanPham = sp.TenSanPham,
                GiaGoc = sp.GiaGoc,
                AnhDaiDien = sp.AnhDaiDien
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new
        {
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            CurrentPage = page,
            Data = products
        };
    }
}