using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class Table
    {
        [Key]
        [Required]
        public int TableNumber { get; set; }
        [Required]
        public int Seats { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}
