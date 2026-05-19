using Restaurant.Models.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }

        [Required]
        public DateTime DateBooked { get; set; } //When you booked
        [Required]
        public int AmountOfGuests { get; set; }
        public string? BookingNotes { get; set; }
        [Required]
        public DateTime StartTime { get; set; } //When you arrive
        [Required]
        public DateTime EndTime { get; set; } //When you leave
        public BookingStatus Status { get; set; }

        //NAV
        public List<Table>? Tables { get; set; }
    }
}