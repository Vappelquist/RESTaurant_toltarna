namespace RESTaurant.Models
{
    internal class Table
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int Seats { get; set; }
        //Nav
        public List<Booking>? Bookings { get; set; }
    }
}
