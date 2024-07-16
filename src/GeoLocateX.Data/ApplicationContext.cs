using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeoLocateX.Data;

public class ApplicationContext : DbContext
{
    public DbSet<BatchProcess> BatchProcesses { get; set; }
    public DbSet<BatchProcessItem> BatchProcessItems { get; set; }
    public DbSet<BatchProcessItemResponse> BatchProcessItemResponses { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchProcess>()
            .HasMany(bp => bp.BatchProcessItems)
            .WithOne(bpi => bpi.BatchProcess)
            .HasForeignKey(bpi => bpi.BatchProcessId);

        modelBuilder.Entity<BatchProcess>(entity =>
        {
            entity.Property(e => e.Status)
            .HasColumnType("tinyint");
        });

        modelBuilder.Entity<BatchProcessItem>(entity =>
        {
            entity.Property(e => e.IpAddress)
                          .HasMaxLength(39);
            entity.Property(e => e.Status)
            .HasColumnType("tinyint");
        });

        modelBuilder.Entity<BatchProcessItemResponse>(entity =>
        {
            entity.Property(e => e.IpAddress)
              .IsRequired()
              .HasMaxLength(39);

            entity.HasIndex(e => e.IpAddress).IsUnique();
        });


        base.OnModelCreating(modelBuilder);

        modelBuilder.RemovePluralizingTableNameConvention();
    }
}

public static class ModelBuilderExtensions
{
    public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.BaseType == null)
            {
                entity.SetTableName(entity.DisplayName());
            }
        }
    }
}
