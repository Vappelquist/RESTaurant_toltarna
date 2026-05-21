using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.Test;

[TestClass]
public class BookingControllerTests
{
    // For in-memory-database:
    private RestaurantDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantDbContext(options);
    }

    [TestMethod]
    public async Task PlaceBooking_WhenFirstNameIsMissing_ReturnBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var ctx = CreateInMemoryDb();
        var controller = new BookingController(ctx, mockBookingService.Object); 

        var request = new PlaceBookingRequest
        {
            FirstName = "",
            LastName = "Svensson",
            Email = "sven@mail.com",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var result = await controller.PlaceBooking(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task PlaceBooking_WhenLastNameIsMissing_ReturnBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var ctx = CreateInMemoryDb();
        var controller = new BookingController(ctx, mockBookingService.Object); 

        var request = new PlaceBookingRequest
        {
            FirstName = "Sven",
            LastName = "",
            Email = "sven@mail.com",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var result = await controller.PlaceBooking(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task PlaceBooking_WhenEmailAndPhoneIsMissing_ReturnBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var ctx = CreateInMemoryDb();
        var controller = new BookingController(ctx, mockBookingService.Object); 

        var request = new PlaceBookingRequest
        {
            FirstName = "Sven",
            LastName = "Svensson",
            Email = "",
            PhoneNumber = "",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var result = await controller.PlaceBooking(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task PlaceBooking_WhenAmountOfGuestsIsMissing_ReturnBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var ctx = CreateInMemoryDb();
        var controller = new BookingController(ctx, mockBookingService.Object); 

        var request = new PlaceBookingRequest
        {
            FirstName = "Sven",
            LastName = "Svensson",
            Email = "sven@mail.com",
            AmountOfGuests = 0,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var result = await controller.PlaceBooking(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}