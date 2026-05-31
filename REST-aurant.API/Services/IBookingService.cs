using Restaurant.API.DTOs;
using Restaurant.Models.Models.Enums;
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
        Task<ServiceResult> ChangeBookingDateAsync(int id, BookingDateChangeDto request);
        Task<ServiceResult> UpdateBookingDetailsAsync(int id, UpdateBookingDetailsRequest request);
        Task<ServiceResult> EditBookingStatusAsync(int id, string request);
    }
}
