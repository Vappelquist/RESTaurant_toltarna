using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Table
    {
        public int Id { get; set; }

        [Required]
        public int TableNumber { get; set; }
        [Required]
        public int Seats { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}
