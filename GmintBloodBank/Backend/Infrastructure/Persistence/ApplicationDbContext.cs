using Domain.Entities.BloodUnits;
using Domain.Entities.Donations;
using Domain.Entities.Donors;
using Domain.Entities.Inventory;
using Domain.Entities.Licensing;
using Domain.Entities.Notifications;
using Domain.Entities.Security;
using Domain.Entities.Tenancy;
using Infrastructure.Configurations.BloodUnits;
using Infrastructure.Configurations.Donations;
using Infrastructure.Configurations.Donors;
using Infrastructure.Configurations.Inventory;
using Infrastructure.Configurations.Licensing;
using Infrastructure.Configurations.Notifications;
using Infrastructure.Configurations.Security;
using Infrastructure.Configurations.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<BloodBank> BloodBanks => Set<BloodBank>();
    public DbSet<StaffCategory> StaffCategories => Set<StaffCategory>();
    public DbSet<Staff> StaffMembers => Set<Staff>();

    public DbSet<BloodType> BloodTypes => Set<BloodType>();
    public DbSet<Gender> Genders => Set<Gender>();
    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<MedicalCondition> MedicalConditions => Set<MedicalCondition>();
    public DbSet<DonorMedicalCondition> DonorMedicalConditions => Set<DonorMedicalCondition>();

    public DbSet<DonationStatus> DonationStatuses => Set<DonationStatus>();
    public DbSet<DonationAppointment> DonationAppointments => Set<DonationAppointment>();
    public DbSet<DonationEvaluation> DonationEvaluations => Set<DonationEvaluation>();
    public DbSet<Donation> Donations => Set<Donation>();

    public DbSet<BloodUnitStatus> BloodUnitStatuses => Set<BloodUnitStatus>();
    public DbSet<BloodComponent> BloodComponents => Set<BloodComponent>();
    public DbSet<BloodUnit> BloodUnits => Set<BloodUnit>();
    public DbSet<BloodScreening> BloodScreenings => Set<BloodScreening>();

    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<InventoryMovementType> InventoryMovementTypes => Set<InventoryMovementType>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    public DbSet<NotificationType> NotificationTypes => Set<NotificationType>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<LicensePlan> LicensePlans => Set<LicensePlan>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<TenantFeature> TenantFeatures => Set<TenantFeature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new BloodBankConfiguration());
        modelBuilder.ApplyConfiguration(new StaffCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new StaffConfiguration());

        modelBuilder.ApplyConfiguration(new BloodTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GenderConfiguration());
        modelBuilder.ApplyConfiguration(new DonorConfiguration());
        modelBuilder.ApplyConfiguration(new MedicalConditionConfiguration());
        modelBuilder.ApplyConfiguration(new DonorMedicalConditionConfiguration());

        modelBuilder.ApplyConfiguration(new DonationStatusConfiguration());
        modelBuilder.ApplyConfiguration(new DonationAppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new DonationEvaluationConfiguration());
        modelBuilder.ApplyConfiguration(new DonationConfiguration());

        modelBuilder.ApplyConfiguration(new BloodUnitStatusConfiguration());
        modelBuilder.ApplyConfiguration(new BloodComponentConfiguration());
        modelBuilder.ApplyConfiguration(new BloodUnitConfiguration());
        modelBuilder.ApplyConfiguration(new BloodScreeningConfiguration());

        modelBuilder.ApplyConfiguration(new StorageLocationConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryMovementTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryMovementConfiguration());

        modelBuilder.ApplyConfiguration(new NotificationTypeConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());

        modelBuilder.ApplyConfiguration(new LicensePlanConfiguration());
        modelBuilder.ApplyConfiguration(new LicenseConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureFlagConfiguration());
        modelBuilder.ApplyConfiguration(new TenantFeatureConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Entities.Common.AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
