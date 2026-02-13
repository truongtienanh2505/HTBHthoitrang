using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories.Models;
using Shop.Infrastructure.Entities;
namespace Shop.Infrastructure.Persistence;


public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>(); 

    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<DanhMucTreeRow> DanhMucTreeRows => Set<DanhMucTreeRow>();

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
         // ================= PRODUCT =================
    modelBuilder.Entity<Product>(e =>
    {
        e.ToTable("SanPham", "dbo");

        e.HasKey(x => x.MaSanPham);

        e.Property(x => x.TenSanPham)
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Slug)
            .HasMaxLength(200)
            .IsRequired();

        e.HasIndex(x => x.Slug).IsUnique();

        e.Property(x => x.GiaGoc)
            .HasColumnType("decimal(18,2)");

        e.Property(x => x.HoatDong)
            .HasDefaultValue(true);

    });
    modelBuilder.Entity<ProductVariant>(e =>
    {
        e.ToTable("BienTheSanPham", "dbo");

        e.HasKey(x => x.MaBienThe);

        e.Property(x => x.SKU)
            .HasMaxLength(50)
            .IsRequired();

        e.HasIndex(x => x.SKU).IsUnique();

        e.Property(x => x.DieuChinhGia)
            .HasColumnType("decimal(18,2)");

        // Không cho trùng variant
        e.HasIndex(x => new { x.MaSanPham, x.MaMauSac, x.MaKichCo })
            .IsUnique();

        // Product relationship
        e.HasOne(v => v.Product)
            .WithMany(p => p.BienThes)
            .HasForeignKey(v => v.MaSanPham)
            .OnDelete(DeleteBehavior.Cascade);

        // Color relationship
    });

        modelBuilder.Entity<DanhMucTreeRow>(e =>
        {
            e.HasNoKey();
            e.ToView(null);
        });
        
    }
}
