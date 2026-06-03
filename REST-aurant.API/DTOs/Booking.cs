using Restaurant.Models.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.DTOs

{
    public class Booking
    {
        public record GetAllBookingResponse
        {
            [Required]
            public int BookingId { get; set; }
            [Required]
            public string? GuestName { get; set; }
            [Required]
            public int AmountOfGuests { get; set; }
            [Required]
            public BookingStatus Status { get; set; }
            public DateOnly DateBooked { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateOnly StartDate { get; set; }
            [Required]
            [DataType(DataType.Time)]
            public TimeOnly StartTime { get; set; }
            public DateOnly EndDate { get; set; }
            public TimeOnly EndTime { get; set; }
            public string? BookingNotes { get; set; }
            public List<int> TableNumbers { get; set; } = new List<int>();
        }

        public record PlaceBookingRequest : UpdateBookingDetailsRequest
        {
            
            [EmailAddress]
            public string? Email { get; set; }
            [Phone]
            public string? PhoneNumber { get; set; }
            [Range(1,28)]
            public int AmountOfGuests { get; set; }
            public DateOnly BookingDate { get; set; }
            public string? StartTime { get; set; }
        }

        public record UpdateBookingDetailsRequest
        {
            [Required]
            public string? FirstName { get; init; }
            [Required]
            public string? LastName { get; init; }
            public string? BookingNotes { get; init; }
        }
    }
}
