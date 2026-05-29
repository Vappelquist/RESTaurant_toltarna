using Restaurant.API.DTOs;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Services
{
    public interface IBookingService
    {
        //Task<(Guest? guest, string? errorMessage)> GetOrCreateGuestAsync(PlaceBookingRequest request);
        Task<ServiceResult> PlaceBookingAsync(PlaceBookingRequest request);
        Task<List<GetAllBookingResponse>> GetAllBookingsAsync();
        Task<GetAllBookingResponse?> GetBookingByIdAsync(int id);
        Task<List<GetAllBookingResponse>> GetWeeklyBookingsAsync(int year, int week);
        Task<List<GetAllBookingResponse>> GetMonthlyBookingsAsync(int year, string month);
        Task<BookingDateDto?> GetBookingDateAsync(int id);
        Task<List<GetAllBookingResponse>> GetBookingsByEmailAsync(string email);
        Task<List<TableStatusDto>> ViewBookingsByTimeAsync(DateOnly date, string time);

        Task<ServiceResult> CancelBookingAsync(int id);
        Task<ServiceResult> ConfirmBookingAsync(int id);
        Task<ServiceResult> CompleteBookingAsync(int id);
        Task<ServiceResult> ChangeBookingDateAsync(int id, BookingDateChangeDto request);
        Task<ServiceResult> UpdateBookingDetailsAsync(int id, UpdateBookingDetailsRequest request);

    }
}
