using Restaurant.Models.Models.Enums;

namespace Restaurant.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? customers { get; set; }
        
        public DateTime DateBooked { get; set; } //When you booked
        public int AmountOfGuests { get; set; }
        public string? BookingNotes { get; set; }
        public DateTime StartTime { get; set; } //When you arrive
        public DateTime EndTime { get; set; } //When you leave
        public BookingStatus status { get; set; }

        //NAV
        public List<Table>? Tables { get; set; }
    }
}