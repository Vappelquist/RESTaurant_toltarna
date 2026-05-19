namespace Restaurant.API.DTOs
{
    public record BookingDateChangeDto(DateOnly NewBookingDate, string? NewStartTime);
}
