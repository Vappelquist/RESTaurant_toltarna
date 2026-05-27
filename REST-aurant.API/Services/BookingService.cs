using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
using System.Globalization;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly RestaurantDbContext _ctx;
        private readonly ITableService _tableService;

        public BookingService(RestaurantDbContext ctx, ITableService tableService)
        {
            _ctx = ctx;
            _tableService = tableService;
        }

        public async Task<(Guest? guest, string? errorMessage)> GetOrCreateGuestAsync(PlaceBookingRequest request)
        {
            var guest = await _ctx.Guests
                .FirstOrDefaultAsync(g => g.Email == request.Email || g.PhoneNumber == request.PhoneNumber);

            if (guest != null)
            {
                if (guest.FirstName != request.FirstName || guest.LastName != request.LastName)
                {
                    return (null, "This email or phone number already belongs to another guest.");
                }
                return (guest, null);
            }

            guest = new Guest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };
            await _ctx.Guests.AddAsync(guest);
            return (guest, null);
        }

        public async Task<string?> PlaceBookingAsync(PlaceBookingRequest request)
        {
            var startTime = TimeOnly.Parse(request.StartTime);
            var startDateTime = request.BookingDate.ToDateTime(startTime);
            var endTime = startDateTime.AddHours(2);

            var (guest, error) = await GetOrCreateGuestAsync(request);
            if (error != null) return error;

            var allocatedTables = await _tableService.AllocateTablesAsync(startDateTime, endTime, request.AmountOfGuests);
            if (!allocatedTables.Any())
                return "This requested time is fully booked, please choose another available time.";

            var booking = new Models.Models.Booking
            {
                Guest = guest,
                AmountOfGuests = request.AmountOfGuests,
                DateBooked = DateTime.Now,
                StartTime = startDateTime,
                EndTime = endTime,
                Status = BookingStatus.Pending,
                BookingNotes = request.BookingNotes,
                Tables = allocatedTables
            };

            await _ctx.Bookings.AddAsync(booking);
            await _ctx.SaveChangesAsync();
            return null;
        }

        public async Task<List<GetAllBookingResponse>> GetAllBookingsAsync()
        {
            return await _ctx.Bookings
                .AsNoTracking()
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
        }

        public async Task<GetAllBookingResponse?> GetBookingByIdAsync(int id)
        {
            var booking = await _ctx.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Tables)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return null;

            return new GetAllBookingResponse
            {
                BookingId = booking.Id,
                GuestName = $"{booking.Guest?.FirstName} {booking.Guest?.LastName}",
                AmountOfGuests = booking.AmountOfGuests,
                Status = booking.Status,
                DateBooked = DateOnly.FromDateTime(booking.DateBooked),
                StartDate = DateOnly.FromDateTime(booking.StartTime),
                StartTime = TimeOnly.FromDateTime(booking.StartTime),
                EndDate = DateOnly.FromDateTime(booking.EndTime),
                EndTime = TimeOnly.FromDateTime(booking.EndTime),
                BookingNotes = booking.BookingNotes,
                TableNumbers = booking.Tables.Select(t => t.TableNumber).ToList()
            };
        }

        public async Task<List<GetAllBookingResponse>> GetWeeklyBookingsAsync(int year, int week)
        {
            var startOfWeek = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            return await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.StartTime >= startOfWeek && b.StartTime < endOfWeek)
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<GetAllBookingResponse>> GetMonthlyBookingsAsync(int year, string month)
        {
            int monthNumber;
            if (!int.TryParse(month, out monthNumber))
            {
                DateTime.TryParse($"1 {month} 2026", out var parsedDate);
                monthNumber = parsedDate.Month;
            }

            var startOfMonth = new DateTime(year, monthNumber, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            return await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.StartTime >= startOfMonth && b.StartTime < endOfMonth)
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
        }

        public async Task<BookingDateDto?> GetBookingDateAsync(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null) return null;

            return new BookingDateDto(
                DateOnly.FromDateTime(booking.DateBooked),
                $"{TimeOnly.FromDateTime(booking.StartTime):HH:mm} - {TimeOnly.FromDateTime(booking.EndTime):HH:mm}"
            );
        }

        public async Task<List<GetAllBookingResponse>> GetBookingsByEmailAsync(string email)
        {
            return await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.Guest != null && b.Guest.Email.ToLower() == email.ToLower())
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<TableStatusDto>> ViewBookingsByTimeAsync(DateOnly date, string time)
        {
            var startTime = TimeOnly.Parse(time);
            var startDateTime = date.ToDateTime(startTime);
            var endTime = startDateTime.AddHours(2);

            var allTables = await _ctx.Tables.ToListAsync();
            var availableTables = await _tableService.GetAvailableTablesAsync(startDateTime, endTime);

            return allTables.Select(t => new TableStatusDto(
                t.TableNumber,
                t.Seats,
                availableTables.Any(at => at.TableNumber == t.TableNumber)
            )).ToList();
        }



        public async Task<string?> CancelBookingAsync(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null) return "notfound";
            if (booking.Status == BookingStatus.Canceled) return "already_canceled";
            booking.Status = BookingStatus.Canceled;
            await _ctx.SaveChangesAsync();
            return null;
        }

        public async Task<string?> ConfirmBookingAsync(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null) return "notfound";
            if (booking.Status == BookingStatus.Confirmed) return "already_confirmed";
            booking.Status = BookingStatus.Confirmed;
            await _ctx.SaveChangesAsync();
            return null;
        }

        public async Task<string?> CompleteBookingAsync(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null) return "notfound";
            if (booking.Status == BookingStatus.Complete) return "already_complete";
            booking.Status = BookingStatus.Complete;
            await _ctx.SaveChangesAsync();
            return null;
        }

        public async Task<string?> ChangeBookingDateAsync(int id, BookingDateChangeDto request)
        {
            var booking = await _ctx.Bookings
                .Include(b => b.Tables)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return "notfound";

            if (!TimeOnly.TryParse(request.NewStartTime, out var startTime))
                return "invalid_time";

            var startDateTime = request.NewBookingDate.ToDateTime(startTime);
            if (startDateTime <= DateTime.Now)
                return "not_in_future";

            var endTime = startDateTime.AddHours(2);
            var allocatedTables = await _tableService.AllocateTablesAsync(startDateTime, endTime, booking.AmountOfGuests, id);
            if (!allocatedTables.Any())
                return "fully_booked";

            booking.StartTime = startDateTime;
            booking.EndTime = endTime;
            booking.Tables = allocatedTables;
            await _ctx.SaveChangesAsync();
            return null;
        }
    }
}