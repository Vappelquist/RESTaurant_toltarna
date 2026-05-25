using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
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


        // This method handles the guest logic:
        // - If the guest already exists, verify the name matches
        // - If the guest does not exist, create a new one
        public async Task<(Guest? guest, string? errorMessage)> GetOrCreateGuestAsync(PlaceBookingRequest request)
        {
            // Search for an existing guest with the same email or phone number
            var guest = await _ctx.Guests
                .FirstOrDefaultAsync(g => g.Email == request.Email || g.PhoneNumber == request.PhoneNumber);

            if (guest != null)
            {
                // Guest already exists — check that the name matches
                // If not, someone else may be attempting to use another person's contact details
                if (guest.FirstName == request.FirstName || guest.LastName == request.LastName)
                {
                    return (null, "This email or phone number already belongs to another guest.");
                }

                // Name matches — return the existing guest
                return (guest, null);
            }

            // No guest found — create a new one
            // We add it to the context but don't save yet (SaveChanges occurs in PlaceBookingAsync)
            guest = new Guest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };
            await _ctx.Guests.AddAsync(guest);

            // Return the new guest without an error message
            return (guest, null);
        }

        public async Task<string?> PlaceBookingAsync(PlaceBookingRequest request)
        {
            // Convert the string "18:30" to a DateTime that the database can store
            var startTime = TimeOnly.Parse(request.StartTime);
            var startDateTime = request.BookingDate.ToDateTime(startTime);
            var endTime = startDateTime.AddHours(2);

            // Retrieve or create the guest using the helper method above
            // If something went wrong, return the error message directly
            var (guest, error) = await GetOrCreateGuestAsync(request);
            if (error != null)
            {
                return error;
            }

            // Ask ITableService to find and allocate available tables for the selected time interval
            // This logic is implemented in TableService
            var allocatedTables = await _tableService.AllocateTablesAsync(startDateTime, endTime, request.AmountOfGuests);

            // If the list is empty there are not enough available seats
            if (!allocatedTables.Any())
            {
                return "This requested time is fully booked, please choose another available time.";
            }

            // Everything is OK — create the booking and link it to the guest and tables
            var booking = new Booking
            {
                Guest = guest,
                AmountOfGuests = request.AmountOfGuests,
                DateBooked = DateTime.Now,
                StartTime = startDateTime,
                EndTime = endTime,
                Status = BookingStatus.Confirmed,
                BookingNotes = request.BookingNotes,
                Tables = allocatedTables
            };

            await _ctx.Bookings.AddAsync(booking);

            // Save everything to the database at once — both the guest (if new) and the booking
            await _ctx.SaveChangesAsync();

            // null means no errors occurred
            return null;
        }
    }
}
