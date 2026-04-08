using Bungalov.Core.Varliklar;
using Microsoft.EntityFrameworkCore;

namespace Bungalov.DataAccess.Baglam;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bungalow> Bungalows { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<BungalowImage> BungalowImages { get; set; }
    public DbSet<Amenity> Amenities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bungalow - Amenity Many-to-Many Configuration
        modelBuilder.Entity<Bungalow>()
            .HasMany(b => b.Amenities)
            .WithMany(a => a.Bungalows)
            .UsingEntity(j => j.ToTable("BungalowAmenities"));
    }
}