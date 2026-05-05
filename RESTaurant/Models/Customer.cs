namespace Restaurant.Models
{
    public class Customer : User
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsEighteen { get; set; }
        public string? Allergies { get; set; }
        public string? Note { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}