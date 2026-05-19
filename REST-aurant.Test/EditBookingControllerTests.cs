using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;

namespace Restaurant.Test;

[TestClass]
public class EditBookingControllerTests
{
    private RestaurantDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantDbContext(options);
    }

    [TestMethod]
    public async Task CancelBooking_WhenBookingDoesNotExist_ReturnNotFound()
    {
        var ctx = CreateInMemoryDb();
        var mockTableService = new Mock<ITableService>();
        var controller = new EditBookingController(ctx, mockTableService.Object);

        var result = await controller.CancelBooking(999);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }
}
