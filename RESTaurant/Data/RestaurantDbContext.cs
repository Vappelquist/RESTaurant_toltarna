using Microsoft.EntityFrameworkCore;
using Restaurant.Models;
using Restaurant.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            modelBuilder.Entity<Guest>().HasData(
            new Guest
            {
                FirstName = "Anna",
                LastName = "Lindqvist",
                Email = "anna.lindqvist@email.se",
                Password = "password123",
                PhoneNumber = "0701234567",
                IsEighteen = true,
                Allergies = "Gluten",
                Note = "Föredrar fönsterbord"
            },
                new Guest
                {
                    FirstName = "Erik",
                    LastName = "Svensson",
                    Email = "erik.svensson@email.se",
                    Password = "password123",
                    PhoneNumber = "0709876543",
                    IsEighteen = true,
                    Allergies = null,
                    Note = null
                },
                new Guest
                {
                    FirstName = "Maria",
                    LastName = "Johansson",
                    Email = "maria.johansson@email.se",
                    Password = "password123",
                    PhoneNumber = "0731122334",
                    IsEighteen = false,
                    Allergies = "Nötter, Laktos",
                    Note = "Allergiker — dubbelkolla alltid"
                }
                );

            // Kopplingstabellen måste seedas explicit med HasData
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Tables)
                .WithMany(t => t.Bookings)
                .UsingEntity(j => j.HasData(
                    new { BookingsId = 1, TablesId = 8 },  // Anna (2 gäster) → bord 15 (Id 8)
                    new { BookingsId = 2, TablesId = 1 },  // Erik (4 gäster) → bord 10 (Id 1)
                    new { BookingsId = 3, TablesId = 2 },  // Maria (6 gäster) → bord 20 (Id 2)
                    new { BookingsId = 3, TablesId = 9 }   // Maria fortsätter  → bord 25 (Id 9)
                ));

            //modelBuilder.Entity<Booking>().HasData(
            //new Booking
            //{
            //    GuestId = 1,
            //    DateBooked = DateTime.Now.AddDays(-3),
            //    AmountOfGuests = 2,
            //    StartTime = DateTime.Today.AddDays(1).AddHours(18),
            //    EndTime = DateTime.Today.AddDays(1).AddHours(20),
            //    BookingNotes = "Jubileum — gärna levande ljus",
            //    Status = BookingStatus.Confirmed,
            //    Tables = new List<Table> { Tables.First(t => t.TableNumber == 15) }
            //},
            //    new Booking
            //    {
            //        GuestId = 2,
            //        DateBooked = DateTime.Now.AddDays(-1),
            //        AmountOfGuests = 4,
            //        StartTime = DateTime.Today.AddDays(2).AddHours(19),
            //        EndTime = DateTime.Today.AddDays(2).AddHours(21),
            //        BookingNotes = null,
            //        Status = BookingStatus.Canceled,
            //        Tables = new List<Table> { Tables.First(t => t.TableNumber == 10) }
            //    },
            //    new Booking
            //    {
            //        GuestId = 3,
            //        DateBooked = DateTime.Now.AddDays(-5),
            //        AmountOfGuests = 6,
            //        StartTime = DateTime.Today.AddDays(3).AddHours(17).AddMinutes(30),
            //        EndTime = DateTime.Today.AddDays(3).AddHours(20),
            //        BookingNotes = "Allergiker i sällskapet",
            //        Status = BookingStatus.Confirmed,
            //        Tables = new List<Table>
            //        {
            //            Tables.First(t => t.TableNumber == 20),
            //            Tables.First(t => t.TableNumber == 25)
            //        }
            //    }
            //    );
        }
    }
}
