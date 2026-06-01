
using global::Restaurant.Models.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models.Models;

namespace Restaurant.API.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .Property(t => t.TableNumber)
                .ValueGeneratedNever();

            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Tables)
                .WithMany(t => t.Bookings)
                .UsingEntity(j => j.ToTable("BookingTables")); // skapar en explicit kopplingstabell

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired(false);

            //modelBuilder.Entity<User>()
            //    .HasDiscriminator<string>("UserType")
            //    .HasValue<Guest>("Guest")
            //    .HasValue<Admin>("Admin");

            base.OnModelCreating(modelBuilder);

            // 1. Tables (Hela restaurangens layout)
            modelBuilder.Entity<Table>().HasData(
                new Table { TableNumber = 1, Seats = 2 },
                new Table { TableNumber = 2, Seats = 2 },
                new Table { TableNumber = 3, Seats = 4 },
                new Table { TableNumber = 4, Seats = 4 },
                new Table { TableNumber = 5, Seats = 6 },
                new Table { TableNumber = 6, Seats = 10 }
            );

            // 2. Guests (Inkluderar nu även Stig)
            modelBuilder.Entity<Guest>().HasData(
                new Guest { Id = 2, Email = "erik@mail.com", Password = "Lösen123", FirstName = "Erik", LastName = "Eriksson", PhoneNumber = "070-4445566", Note = "Fyller år!" },
                new Guest { Id = 3, Email = "stig@mail.com", Password = "Lösen123", FirstName = "Stig", LastName = "Stigsson", PhoneNumber = "070-7778899", Allergies = "Laktos", Note = "Barnstol behövs" },
                new Guest { Id = 1, Email = "anna@mail.com", Password = "Lösen123", FirstName = "Anna", LastName = "Andersson", PhoneNumber = "070-1112233", Allergies = "Nötter", Note = "Vill sitta vid fönstret" }
            );

            // 3. Bookings (Tre olika scenarier)
            modelBuilder.Entity<Booking>().HasData(
                // Anna: Bekräftad
                new Booking { Id = 1, GuestId = 1, DateBooked = new DateTime(2026, 05, 08), AmountOfGuests = 2, StartTime = new DateTime(2026, 05, 08, 18, 0, 0), EndTime = new DateTime(2026, 05, 08, 20, 0, 0), Status = BookingStatus.Confirmed, BookingNotes = "Dejt-kväll" },

                // Erik: Avbokad (Här testar vi status-enumen)
                new Booking { Id = 2, GuestId = 2, DateBooked = new DateTime(2026, 05, 08), AmountOfGuests = 4, StartTime = new DateTime(2026, 05, 10, 19, 0, 0), EndTime = new DateTime(2026, 05, 10, 21, 0, 0), Status = BookingStatus.Canceled, BookingNotes = "Inställt pga sjukdom" },

                // Stig: Stort sällskap (Här testar vi flera bord)
                new Booking { Id = 3, GuestId = 3, DateBooked = new DateTime(2026, 05, 08), AmountOfGuests = 12, StartTime = new DateTime(2026, 05, 15, 17, 0, 0), EndTime = new DateTime(2026, 05, 15, 20, 0, 0), Status = BookingStatus.Confirmed, BookingNotes = "Släktträff" }
            );

            // 4. Kopplingstabellen (Många-till-många)
            // Namnet i citattecken måste matcha namnet EF Core gett tabellen (oftast "BookingTable" eller "BookingTables")
            modelBuilder.Entity("BookingTable").HasData(
                new { BookingsId = 1, TablesTableNumber = 1 },
                new { BookingsId = 3, TablesTableNumber = 5 },
                new { BookingsId = 3, TablesTableNumber = 6 }
             );

            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 4, Email = "admin@mail.com", Password = "Admin123" }

);
        }
    }
}
