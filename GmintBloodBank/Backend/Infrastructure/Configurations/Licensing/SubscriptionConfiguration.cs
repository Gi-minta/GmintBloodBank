using Domain.Entities.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Licensing;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TransactionId).HasMaxLength(100);
        builder.HasOne(e => e.License).WithMany(l => l.Subscriptions).HasForeignKey(e => e.LicenseId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
