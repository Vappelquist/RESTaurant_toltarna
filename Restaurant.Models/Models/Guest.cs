using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class Guest : User
    {
        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Allergies { get; set; }
        public string? Note { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}