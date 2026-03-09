using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories.Models;
using Shop.Application.Auth.Models; 
using Shop.Application.Products.Models;
using Shop.Application.Orders.Models;

namespace Shop.Infrastructure.Persistence;

public class ShopDbContext : DbContext
{
    // Trong class ShopDbContext
    public DbSet<KhuyenMai> KhuyenMais => Set<KhuyenMai>();
    
    public DbSet<SanPhamKhuyenMai> SanPhamKhuyenMais => Set<SanPhamKhuyenMai>();
     public DbSet<SanPham> SanPhams { get; set; }
    public DbSet<DanhMuc> DanhMucs { get; set; } = null!;
    public DbSet<Product> Products { get; set; } 
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductVariant> BienTheSanPhams { get; set; }
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<DanhMucTreeRow> DanhMucTreeRows => Set<DanhMucTreeRow>();
    
    public DbSet<SanPham> SanPhams => Set<SanPham>();
    
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>(); 

    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<LichSuDonHang> LichSuDonHangs => Set<LichSuDonHang>();
    
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

        // Cấu hình bảng NguoiDung
        modelBuilder.Entity<NguoiDung>(e =>
        {
            e.ToTable("NguoiDung", "dbo");
            e.HasKey(x => x.MaNguoiDung);
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
