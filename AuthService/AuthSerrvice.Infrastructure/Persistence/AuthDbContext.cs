using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FullName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.OwnsOne(u => u.Email, owned =>
            {
                owned.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired()
                     .HasMaxLength(150);

                owned.HasIndex(e => e.Value)
                     .IsUnique();
            });

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.Role)
               .HasConversion<string>()
               .IsRequired();

            entity.Property(u => u.CreatedAt)
                  .IsRequired();

            entity.Property(u => u.IsActive)
                  .IsRequired();
            entity.Property(u => u.IsLocked)
                .IsRequired();

            entity.Property(u => u.FailedLoginAttempts)
                  .IsRequired();

            entity.Property(u => u.LockoutEnd);
        });

        base.OnModelCreating(modelBuilder);
    }
}
