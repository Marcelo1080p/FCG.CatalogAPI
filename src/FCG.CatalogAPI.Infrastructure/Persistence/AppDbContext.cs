using FCG.CatalogAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.CatalogAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<UserGame> UserGames => Set<UserGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(e =>
        {
            e.HasKey(g => g.Id);
            e.Property(g => g.Title).IsRequired().HasMaxLength(200);
            e.Property(g => g.Description).IsRequired().HasMaxLength(1000);
            e.Property(g => g.Price).HasPrecision(18, 2);
            e.Property(g => g.DiscountPercentage).HasPrecision(5, 2);
        });

        modelBuilder.Entity<UserGame>(e =>
        {
            e.HasKey(ug => ug.Id);
            e.HasIndex(ug => new { ug.UserId, ug.GameId }).IsUnique();
            e.Property(ug => ug.PricePaid).HasPrecision(18, 2);
            e.HasOne(ug => ug.Game)
             .WithMany()
             .HasForeignKey(ug => ug.GameId);
        });
    }
}
