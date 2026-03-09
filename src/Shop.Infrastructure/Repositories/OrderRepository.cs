using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shop.Application.Interfaces;
using Shop.Application.Products;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopDbContext _context;
        private IDbContextTransaction? _transaction;

        public OrderRepository(ShopDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task<Product?> GetProductForUpdateAsync(int productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.MaSanPham == productId);
        }

        public async Task<KhuyenMai?> GetActivePromotionAsync(int productId, DateTime now)
        {
            return await _context.SanPhamKhuyenMais
                .Include(x => x.KhuyenMai)
                .Where(x => x.MaSanPham == productId
                            && x.KhuyenMai.KichHoat
                            && x.KhuyenMai.BatDau <= now
                            && x.KhuyenMai.KetThuc >= now)
                .Select(x => x.KhuyenMai)
                .FirstOrDefaultAsync();
        }

        public async Task<List<KhuyenMai>> GetActivePromotionsAsync(int productId, DateTime now)
        {
            return await _context.SanPhamKhuyenMais
                .Include(x => x.KhuyenMai)
                .Where(x => x.MaSanPham == productId
                    && x.KhuyenMai != null
                    && x.KhuyenMai.KichHoat
                    && x.KhuyenMai.BatDau <= now
                    && x.KhuyenMai.KetThuc >= now)
                .Select(x => x.KhuyenMai!)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}