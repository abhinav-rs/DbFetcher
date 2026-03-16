using DbFetcher.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace DbFetcher.Data;

public class InvoiceDbContext : DbContext
{
    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.InvoiceAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.PaymentAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.InvoiceNumber)     .HasMaxLength(50);
            entity.Property(e => e.DistributorName)   .HasMaxLength(200);
            entity.Property(e => e.LocationId)        .HasMaxLength(100);
            entity.Property(e => e.LocationName)      .HasMaxLength(200);
            entity.Property(e => e.Status)            .HasMaxLength(50);
            entity.Property(e => e.PaymentType)       .HasMaxLength(100);
            entity.Property(e => e.PaymentMethod)     .HasMaxLength(100);
            entity.Property(e => e.PoNumber)          .HasMaxLength(100);
            entity.Property(e => e.ReferenceNumber)   .HasMaxLength(100);
            entity.Property(e => e.ApprovedBy)        .HasMaxLength(100);
            entity.Property(e => e.ApprovedByFullName).HasMaxLength(200);
            entity.Property(e => e.ScheduledByFullName).HasMaxLength(200);
            entity.Property(e => e.InvoiceSource)     .HasMaxLength(100);
        });
    }
}
