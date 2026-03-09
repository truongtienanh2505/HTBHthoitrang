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
        var query = _context.SanPhams.AsNoTracking().Where(sp => sp.HoatDong == true);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            query = query.Where(sp => EF.Functions.FreeText(sp.TenSanPham, req.Keyword) 
                                   || EF.Functions.FreeText(sp.MoTa, req.Keyword));
        }

        if (req.MinPrice.HasValue) query = query.Where(sp => sp.GiaGoc >= req.MinPrice.Value);
        if (req.MaxPrice.HasValue) query = query.Where(sp => sp.GiaGoc <= req.MaxPrice.Value);

        var totalItems = await query.CountAsync();

        var products = await query
            .Select(sp => new SearchResult 
            {
                MaSanPham = sp.MaSanPham,
                TenSanPham = sp.TenSanPham,
                GiaGoc = sp.GiaGoc,
                AnhDaiDien = sp.AnhDaiDien
            })
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync();

        return new 
        { 
            TotalItems = totalItems, 
            TotalPages = (int)Math.Ceiling(totalItems / (double)req.PageSize),
            CurrentPage = req.Page,
            Data = products 
        };
    }
}