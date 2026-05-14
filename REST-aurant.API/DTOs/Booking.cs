using Restaurant.Models.Models.Enums;

namespace REST_aurant.API.DTOs

{
    public class Booking
    {
        public class GetAllBookingResponse
        {
            public int BookingId { get; set; }
            public string? GuestName { get; set; }
            public int AmountOfGuests { get; set; }
            public BookingStatus Status { get; set; }
            public DateOnly DateBooked { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string? BookingNotes { get; set; }
            public List<int> TableNumbers { get; set; } = new List<int>();
        }
    }
}
