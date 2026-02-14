using Microsoft.EntityFrameworkCore;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Enums;

namespace SupplierService.Infrastructure.Persistence;

public class SupplierDbContext : DbContext
{
    public SupplierDbContext(DbContextOptions<SupplierDbContext> options)
        : base(options)
    {
    }

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.OwnsOne(x => x.Cnpj, owned =>
            {
                owned.Property(p => p.Value)
                     .HasColumnName("Cnpj")
                     .IsRequired()
                     .HasMaxLength(14);

                owned.HasIndex(p => p.Value)
                     .IsUnique();
            });

            entity.OwnsOne(x => x.Email, owned =>
            {
                owned.Property(p => p.Value)
                     .HasColumnName("Email")
                     .IsRequired()
                     .HasMaxLength(150);
            });

            entity.OwnsOne(x => x.Phone, owned =>
            {
                owned.Property(p => p.Value)
                     .HasColumnName("Phone")
                     .IsRequired()
                     .HasMaxLength(20);
            });

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.Property(x => x.UpdatedAt)
                  .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(x => x.Description)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(x => x.Price)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Sku)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(x => x.Sku)
                  .IsUnique();

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.Property(x => x.UpdatedAt)
                  .IsRequired();
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity)
                  .IsRequired();

            entity.Property(x => x.TotalAmount)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Status)
                  .IsRequired()
                  .HasConversion<int>();

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.Property(x => x.UpdatedAt)
                  .IsRequired();

            entity.Property(x => x.CreatedBy)
                  .IsRequired();

            entity.HasOne<Supplier>()
                  .WithMany()
                  .HasForeignKey(x => x.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Product>()
                  .WithMany()
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
