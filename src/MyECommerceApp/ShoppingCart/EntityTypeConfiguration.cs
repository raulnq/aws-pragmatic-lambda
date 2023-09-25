using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyECommerceApp.Infrastructure.EntityFramework;

namespace MyECommerceApp.ShoppingCart;

public class EntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
{
    public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder
            .ToTable(Tables.ShoppingCartItems);

        builder
            .HasKey(s => s.ShoppingCartItemId);

        builder
            .Property(s => s.Quantity)
            .HasColumnType("decimal(19,4)");
    }
}
