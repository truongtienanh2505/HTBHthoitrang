using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories.Models;
using Shop.Application.Auth.Models; 
using Shop.Application.Products.Models;
using Shop.Application.Orders.Models;

namespace Shop.Infrastructure.Persistence;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<DanhMucTreeRow> DanhMucTreeRows => Set<DanhMucTreeRow>();
    
    public DbSet<SanPham> SanPhams => Set<SanPham>();
    
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>(); 

    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<LichSuDonHang> LichSuDonHangs => Set<LichSuDonHang>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DanhMuc>(e =>
        {
            e.ToTable("DanhMuc", "dbo");
            e.HasKey(x => x.MaDanhMuc);

            e.Property(x => x.TenDanhMuc).HasMaxLength(100).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();

            e.Property(x => x.HoatDong).HasDefaultValue(true);

            e.HasOne(x => x.DanhMucCha)
             .WithMany(x => x.DanhMucCon)
             .HasForeignKey(x => x.MaDanhMucCha)
             .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DanhMucTreeRow>(e =>
        {
            e.HasNoKey();
            e.ToView(null);
        });

        // Cấu hình bảng NguoiDung
        modelBuilder.Entity<NguoiDung>(e =>
        {
            e.ToTable("NguoiDung", "dbo");
            e.HasKey(x => x.MaNguoiDung);
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
