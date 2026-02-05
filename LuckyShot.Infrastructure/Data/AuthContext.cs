using LuckyShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Data;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public const string DatabaseSchema = "auth";
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DatabaseSchema);
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<string>();
            entity.Property(e => e.EmailConfirmationTokenHash).HasMaxLength(64).IsRequired(false);
            entity.Property(e => e.EmailConfirmationExpiresAt).IsRequired(false);
        });
    }
}