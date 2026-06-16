using Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Security;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Username).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(150).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
        builder.HasIndex(e => e.Username).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasIndex(e => e.Email).IsUnique().HasFilter("\"IsDeleted\" = FALSE");
        builder.HasOne(e => e.Role).WithMany(r => r.Users).HasForeignKey(e => e.RoleId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
