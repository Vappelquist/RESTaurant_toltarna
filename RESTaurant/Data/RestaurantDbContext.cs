using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Models.Data
{
    public class RestaurantDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();
        }
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base (options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<BookingStatus> BookingStatuses{ get; set; }
    }
}
