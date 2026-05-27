using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class Table
    {
        [Key]
        public int TableNumber { get; set; }
        [Required]
        [Range(2, 10)]
        public int Seats { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}
