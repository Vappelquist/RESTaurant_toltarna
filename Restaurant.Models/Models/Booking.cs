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
        [DataType(DataType.Date)]
        public DateTime DateBooked { get; set; } //When you booked
        [Required]
        [Range(1, 28)]
        public int AmountOfGuests { get; set; }
        public string? BookingNotes { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; } //When you arrive
        [Required]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; } //When you leave
        public BookingStatus Status { get; set; }

        //NAV
        public List<Table>? Tables { get; set; }
    }
}