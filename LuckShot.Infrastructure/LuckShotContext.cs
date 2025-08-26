using LuckShot.Domain;
using Microsoft.EntityFrameworkCore;

namespace LuckShot.Infrastructure;

public class LuckyShotContext(DbContextOptions<LuckyShotContext> options) : DbContext(options)
{
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Uuid);
            entity.HasOne(e => e.CurrentSeason)
                .WithMany()
                .HasForeignKey(e => e.CurrentSeasonId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.ExternalId).IsUnique();
        });
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.HomeTeam)
                .WithMany()
                .HasForeignKey(e => e.HomeTeamId);
            entity.HasOne(e => e.AwayTeam)
                .WithMany()
                .HasForeignKey(e => e.AwayTeamId);
            entity.HasOne(e => e.Season)
                .WithMany()
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
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