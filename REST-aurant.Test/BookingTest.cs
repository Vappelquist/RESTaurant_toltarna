using Microsoft.EntityFrameworkCore;
using Restaurant.Models.Models;

namespace Restaurant.Test;

[TestClass]
public class BookingTest
{

    private Booking _booking;

    public BookingTest()
    {
        _booking = new Booking();
    }

    // In-memory-database:
    //private RestaurantDbContext CreateInMemoryDb()
    //{
    //    var options = new DbContextOptionsBuilder<RestaurantDbContext>()
    //        .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //        .Options;
    //    return new RestaurantDbContext(options);
    //}

    [TestMethod]
    public void TestMethod1()
    {
    }
}
