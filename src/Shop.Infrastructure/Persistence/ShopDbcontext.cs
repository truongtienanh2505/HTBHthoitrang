using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories.Models;
using Shop.Application.Products;

namespace Shop.Infrastructure.Persistence;

public class ShopDbContext : DbContext
{
    // Trong class ShopDbContext
    public DbSet<DanhMuc> DanhMucs { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<ProductVariant> BienTheSanPhams { get; set; }
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<DanhMucTreeRow> DanhMucTreeRows => Set<DanhMucTreeRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ToTable("SanPham");
        modelBuilder.Entity<ProductVariant>().ToTable("BienTheSanPham");
        modelBuilder.Entity<DanhMuc>().ToTable("DanhMuc");
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
        modelBuilder.Entity<ProductVariant>()
        .Property(p => p.GiaBienThe)
        .HasColumnType("decimal(18,2)");
    }
}
