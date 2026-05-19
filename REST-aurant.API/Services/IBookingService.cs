using Restaurant.Models.Models;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Services
{
    public interface IBookingService
    {
        // Returns a tuple with either a guest or an error message
        Task<(Guest? guest, string? errorMessage)> GetOrCreateGuestAsync(PlaceBookingRequest request);

        // Returns null if success, if not an error message as a string
        Task<string?> PlaceBookingAsync(PlaceBookingRequest request);
    }
}
