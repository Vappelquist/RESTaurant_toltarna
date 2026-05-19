namespace REST_aurant.API.DTOs
{
    public record BookingDateChangeDto(DateOnly NewBookingDate, string? NewStartTime);
}
