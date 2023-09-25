using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyECommerceApp.Infrastructure;
using MyECommerceApp.Infrastructure.EntityFramework;

namespace MyECommerceApp.Orders;

public class EntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .ToTable(Tables.Orders);

        builder
            .HasKey(o => o.OrderId);

        builder
            .Property(cr => cr.Status)
            .HasConversion(status => status.ToString(), value => value.ToEnum<OrderStatus>());

        builder
            .Property(cr => cr.PaymentMethod)
            .HasConversion(status => status.ToString(), value => value.ToEnum<PaymentMethod>());

        builder
            .Property(oi => oi.Total)
            .HasColumnType("decimal(19,4)");

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(o => o.OrderId);
    }
}

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Order.Item>
{
    public void Configure(EntityTypeBuilder<Order.Item> builder)
    {
        builder
            .ToTable(Tables.OrderItems);

        builder
            .HasKey(oi => new { oi.OrderId, oi.ProductId });

        builder
            .Property(oi => oi.Price)
            .HasColumnType("decimal(19,4)");

        builder
            .Property(oi => oi.Quantity)
            .HasColumnType("decimal(19,4)");

    }
}
