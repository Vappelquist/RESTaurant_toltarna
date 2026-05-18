using Restaurant.Models.Models.Enums;

namespace REST_aurant.API.DTOs

{
    public class Booking
    {
        public record GetAllBookingResponse
        {
            public int BookingId { get; set; }
            public string? GuestName { get; set; }
            public int AmountOfGuests { get; set; }
            public BookingStatus Status { get; set; }
            public DateOnly DateBooked { get; set; }
            public DateOnly StartDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public DateOnly EndDate { get; set; }
            public TimeOnly EndTime { get; set; }
            public string? BookingNotes { get; set; }
            public List<int> TableNumbers { get; set; } = new List<int>();
        }

        public record PlaceBookingRequest
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public int AmountOfGuests { get; set; }
            public DateTime StartTime { get; set; }
            public string? BookingNotes { get; set; }
        }
    }
}
