using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyECommerceApp.Infrastructure;
using MyECommerceApp.Infrastructure.EntityFramework;

namespace MyECommerceApp.ClientRequests;

public class EntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> builder)
    {
        builder
            .ToTable(Tables.ClientRequests);

        builder
            .HasKey(cr => cr.ClientRequestId);

        builder
            .Property(cr => cr.Status)
            .HasConversion(status => status.ToString(), value =>value.ToEnum<ClientRequestStatus>());
    }
}
