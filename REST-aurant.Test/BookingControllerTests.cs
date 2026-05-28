using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.Test;

[TestClass]
public class BookingControllerTests
{
    

    // PlaceBooking-tests ---------------------------------------------------------------V
    [TestMethod]
    public async Task PlaceBooking_WhenFirstNameIsMissing_ReturnBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var controller = new BookingController(mockBookingService.Object); 

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
        var controller = new BookingController(mockBookingService.Object);

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
        var controller = new BookingController(mockBookingService.Object);

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
        var controller = new BookingController(mockBookingService.Object);

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





    [TestMethod]
    public async Task PlaceBooking_WhenRestaurantIsFull_ReturnsError()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        var table = new Table { TableNumber = 1, Seats = 4 };
        ctx.Tables.Add(table);
        await ctx.SaveChangesAsync();

        ctx.Bookings.Add(new Booking
        {
            AmountOfGuests = 2,
            StartTime = DateTime.Now.AddDays(1).Date.AddHours(18),
            EndTime = DateTime.Now.AddDays(1).Date.AddHours(20),
            DateBooked = DateTime.Now,
            Status = BookingStatus.Confirmed,
            Guest = new Guest
            {
                FirstName = "Sven",
                LastName = "Svensson",
                Email = "sven@mail.com",
                PhoneNumber = "0701234567"
            },
            Tables = new List<Table> { table }
        });
        await ctx.SaveChangesAsync();

        var tableService = new TableService(ctx);
        var bookingService = new BookingService(ctx, tableService);

        // Place new booking at the same time-slot as the existing one:
        var newBooking = new PlaceBookingRequest
        {
            FirstName = "Lars",
            LastName = "Larsson",
            Email = "lars@mail.com",
            PhoneNumber = "0709876543",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var error = await bookingService.PlaceBookingAsync(newBooking);

        // Assert
        Assert.IsNotNull(error);

        // Returns error-message as the restaurant is fully booked:
        Assert.AreEqual("This requested time is fully booked, please choose another available time.", error);
    }

    // PlaceBooking-tests ---------------------------------------------------------------^



    // GetMonthlyBookings-tests ---------------------------------------------------------------V

    // GetMonthlyBookings-tests ---------------------------------------------------------------^



    // GetWeeklyBookings-tests ---------------------------------------------------------------V

    // GetWeeklyBookings-tests ---------------------------------------------------------------^


}