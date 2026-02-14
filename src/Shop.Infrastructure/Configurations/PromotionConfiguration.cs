using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Infrastructure.Entities;

namespace Shop.Infrastructure.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenKhuyenMai)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.PhanTramGiam)
                   .HasColumnType("decimal(5,2)");

            builder.Property(x => x.TrangThai)
                   .HasDefaultValue(true);
        }
    }
}
