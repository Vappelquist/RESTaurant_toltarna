using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant.Models
{
    internal class Booking
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public DateTime DateBooked { get; set; }
        public int AmountOfGuests { get; set; }
        public string? BookingNotes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
