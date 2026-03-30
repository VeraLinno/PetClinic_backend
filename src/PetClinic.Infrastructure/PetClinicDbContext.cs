using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

// UTC DateTime Converters for PostgreSQL
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter() : base(
        v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : (DateTime?)null,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null)
    {
    }
}

public class PetClinicDbContext : DbContext
{
    public PetClinicDbContext(DbContextOptions<PetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Owner> Owners { get; set; } = default!;
    public DbSet<Pet> Pets { get; set; } = default!;
    public DbSet<Veterinarian> Veterinarians { get; set; } = default!;
    public DbSet<Appointment> Appointments { get; set; } = default!;
    public DbSet<Visit> Visits { get; set; } = default!;
    public DbSet<Prescription> Prescriptions { get; set; } = default!;
    public DbSet<MedicationStock> MedicationStocks { get; set; } = default!;
    public DbSet<InventoryReorder> InventoryReorders { get; set; } = default!;
    public DbSet<Invoice> Invoices { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<VetUnavailability> VetUnavailabilities { get; set; } = default!;
    public DbSet<Translation> Translations { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configure all DateTime properties to use UTC for PostgreSQL
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder
            .Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email for Owner
        modelBuilder.Entity<Owner>()
            .HasIndex(o => o.Email)
            .IsUnique();

        // Unique email for Veterinarian
        modelBuilder.Entity<Veterinarian>()
            .HasIndex(v => v.Email)
            .IsUnique();

        // Relations
        modelBuilder.Entity<Pet>()
            .HasOne(p => p.Owner)
            .WithMany(o => o.Pets)
            .HasForeignKey(p => p.OwnerId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Pet)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PetId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian)
            .WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VeterinarianId);

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Appointment)
            .WithOne(a => a.Visit)
            .HasForeignKey<Visit>(v => v.AppointmentId);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Visit)
            .WithMany(v => v.Prescriptions)
            .HasForeignKey(p => p.VisitId);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Visit)
            .WithOne(v => v.Invoice)
            .HasForeignKey<Invoice>(i => i.VisitId);

        modelBuilder.Entity<InventoryReorder>()
            .HasOne(r => r.MedicationStock)
            .WithMany(m => m.Reorders)
            .HasForeignKey(r => r.MedicationStockId);

        modelBuilder.Entity<MedicationStock>()
            .Property(m => m.UnitPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Owner)
            .WithMany(o => o.RefreshTokens)
            .HasForeignKey(rt => rt.OwnerId);

        modelBuilder.Entity<VetUnavailability>()
            .HasOne(vu => vu.Veterinarian)
            .WithMany(v => v.Unavailabilities)
            .HasForeignKey(vu => vu.VeterinarianId);

        // Enum conversion for AppointmentStatus
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        // Roles as JSON for Owner
        modelBuilder.Entity<Owner>()
            .Property(o => o.Roles)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        // Translation entity configuration
        modelBuilder.Entity<Translation>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Translation>()
            .Property(t => t.LanguageCode)
            .HasMaxLength(10);

        modelBuilder.Entity<Translation>()
            .Property(t => t.Category)
            .HasMaxLength(100);

        modelBuilder.Entity<Translation>()
            .HasIndex(t => new { t.LanguageCode, t.IsActive })
            .HasDatabaseName("IX_Translations_LanguageCode_IsActive");

        modelBuilder.Entity<Translation>()
            .HasIndex(t => new { t.LanguageCode, t.Category, t.IsActive })
            .HasDatabaseName("IX_Translations_LanguageCode_Category_IsActive");

        modelBuilder.Entity<Translation>()
            .HasIndex(t => new { t.LanguageCode, t.Category, t.Key })
            .IsUnique()
            .HasDatabaseName("IX_Translations_LanguageCode_Category_Key");

        modelBuilder.Entity<Translation>()
            .HasIndex(t => t.Category)
            .HasDatabaseName("IX_Translations_Category");

        modelBuilder.Entity<Translation>()
            .HasIndex(t => t.Key)
            .HasDatabaseName("IX_Translations_Key");
    }
}