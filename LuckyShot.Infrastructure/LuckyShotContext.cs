using LuckyShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure;

public class LuckyShotContext(DbContextOptions<LuckyShotContext> options) : DbContext(options)
{
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("master");
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.CurrentSeason)
                .WithMany()
                .HasForeignKey(e => e.CurrentSeasonId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Result).HasConversion<string>();
            entity.HasOne(e => e.HomeTeam)
                .WithMany()
                .HasForeignKey(e => e.HomeTeamId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.AwayTeam)
                .WithMany()
                .HasForeignKey(e => e.AwayTeamId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Season)
                .WithMany(s => s.Matches)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Competition)
                .WithMany()
                .HasForeignKey(e => e.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
    }
}