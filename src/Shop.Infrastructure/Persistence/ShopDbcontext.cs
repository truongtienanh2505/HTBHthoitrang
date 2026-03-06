using Microsoft.EntityFrameworkCore;
using Shop.Application.Categories.Models;
using Shop.Application.Products;
using Shop.Infrastructure.Products;
using Shop.Application.Models;
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
        modelBuilder.Entity<KhuyenMai>(entity =>
    {
        entity.ToTable("KhuyenMai", "dbo");
        entity.HasKey(e => e.MaKhuyenMai);

    // Ràng buộc loại giảm giá theo SQL
        entity.Property(e => e.LoaiGiamGia).IsRequired().HasMaxLength(20);
    
    // Ràng buộc giá trị giảm (0 < GiaTriGiam <= 100 nếu là PERCENTAGE)
        entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18,2)");
        entity.Property(e => e.GiamToiDa).HasPrecision(18, 2);
});

        modelBuilder.Entity<SanPhamKhuyenMai>(entity =>
    {
        entity.ToTable("SanPhamKhuyenMai", "dbo");
        entity.HasKey(e => e.Ma);

    // Cấu hình quan hệ với Khuyến mãi
        entity.HasOne(d => d.KhuyenMai)
            .WithMany(p => p.SanPhamKhuyenMais) 
            .HasForeignKey(d => d.MaKhuyenMai)
            .OnDelete(DeleteBehavior.Cascade);

    // Cấu hình quan hệ với Sản phẩm
        entity.HasOne(d => d.SanPham)
            .WithMany() // Để trống nếu bên SanPham.cs chưa có ICollection<SanPhamKhuyenMai>
            .HasForeignKey(d => d.MaSanPham)
            .OnDelete(DeleteBehavior.Cascade);
});
        modelBuilder.Entity<DieuKienKhuyenMai>(entity =>
    {
        entity.HasKey(e => e.Ma); // Xác định Ma là khóa chính
        entity.ToTable("DieuKienKhuyenMai", "dbo");

        entity.HasOne(d => d.KhuyenMai)
            .WithMany(p => p.DieuKiens)
            .HasForeignKey(d => d.MaKhuyenMai)
            .OnDelete(DeleteBehavior.Cascade);
            
});
    modelBuilder.Entity<ProductVariant>()
        .ToTable("ProductVariants");

    base.OnModelCreating(modelBuilder);
}
}
