using Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Tenancy;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Country).HasMaxLength(100).IsRequired();
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.City).HasMaxLength(100).IsRequired();
        builder.Property(e => e.AddressLine1).HasMaxLength(250).IsRequired();
        builder.Property(e => e.AddressLine2).HasMaxLength(250);
        builder.Property(e => e.PostalCode).HasMaxLength(20);
        builder.Property(e => e.Latitude).HasColumnType("decimal(10,7)");
        builder.Property(e => e.Longitude).HasColumnType("decimal(10,7)");
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
