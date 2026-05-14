using Bungalov.Core.Varliklar;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bungalov.DataAccess.Baglam;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bungalow> Bungalows { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<BungalowImage> BungalowImages { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bungalow - Amenity Many-to-Many Configuration
        modelBuilder.Entity<Bungalow>()
            .HasMany(b => b.Amenities)
            .WithMany(a => a.Bungalows)
            .UsingEntity(j => j.ToTable("BungalowAmenities"));

        modelBuilder.Entity<Bungalow>()
            .Property(b => b.PricePerNight)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Reservation>()
            .Property(r => r.TotalPrice)
            .HasColumnType("decimal(18,2)");

        // Review Cycle Fix
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Reservation)
            .WithMany()
            .HasForeignKey(r => r.ReservationId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.AppUser)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.AppUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}