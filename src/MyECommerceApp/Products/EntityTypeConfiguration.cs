using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyECommerceApp.Infrastructure.EntityFramework;

namespace MyECommerceApp.Products;

public class EntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable(Tables.Products);

        builder
            .HasKey(p => p.ProductId);

        builder
            .Property(p => p.Price)
            .HasColumnType("decimal(19,4)");
    }
}
