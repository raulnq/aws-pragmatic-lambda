using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyECommerceApp.Infrastructure.EntityFramework;

namespace MyECommerceApp.Clients;

public class EntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder
            .ToTable(Tables.Clients);

        builder
            .HasKey(cr => cr.ClientId);
    }
}
