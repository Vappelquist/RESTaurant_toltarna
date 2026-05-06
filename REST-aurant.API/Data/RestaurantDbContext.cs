using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace REST_aurant.API.Data
{

    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Tables)
                .WithMany(t => t.Bookings)
                .UsingEntity(j => j.ToTable("BookingTables")); // skapar en explicit koppllingstabell
        }
    }
}

