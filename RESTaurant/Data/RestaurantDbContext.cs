using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Data 
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Tables)
                .WithMany(t => t.Bookings)
                .UsingEntity(j => j.ToTable("BookingTables")); // skapar en explicit kopplingstabell

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>().HasData(
                new Table { Id = 1, TableNumber = 10, Seats = 4 },
                new Table { Id = 2, TableNumber = 20, Seats = 4 },
                new Table { Id = 3, TableNumber = 30, Seats = 4 },
                new Table { Id = 4, TableNumber = 40, Seats = 4 },
                new Table { Id = 5, TableNumber = 50, Seats = 4 },
                new Table { Id = 6, TableNumber = 60, Seats = 4 },
                new Table { Id = 7, TableNumber = 70, Seats = 4 },
                new Table { Id = 8, TableNumber = 15, Seats = 2 },
                new Table { Id = 9, TableNumber = 25, Seats = 2 },
                new Table { Id = 10, TableNumber = 35, Seats = 2 },
                new Table { Id = 11, TableNumber = 45, Seats = 2 },
                new Table { Id = 12, TableNumber = 55, Seats = 2 },
                new Table { Id = 13, TableNumber = 65, Seats = 2 },
                new Table { Id = 14, TableNumber = 75, Seats = 2 }
                );
        }
    }
}
