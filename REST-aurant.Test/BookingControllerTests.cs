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
    // For in-memory-database:
    private RestaurantDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantDbContext(options);
    }

    // PlaceBooking-tests ---------------------------------------------------------------V
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

    [TestMethod]
    public async Task PlaceBooking_IfEndTimeIsTwoHoursAfterStartTime_ReturnTrue()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        var mockTableService = new Mock<ITableService>();
        mockTableService
        .Setup(s => s.AllocateTablesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int?>())) // It.IsAny<T>() = Mock-specific, dont worry about what value it is, always match.
        .ReturnsAsync(new List<Table> { new Table { TableNumber = 1, Seats = 4 } });

        var service = new BookingService(ctx, mockTableService.Object);

        var request = new PlaceBookingRequest
        {
            FirstName = "Sven",
            LastName = "Svensson",
            Email = "sven@mail.com",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        await service.PlaceBookingAsync(request);

        // Assert
        var booking = await ctx.Bookings.FirstOrDefaultAsync();
        Assert.IsNotNull(booking);
        Assert.AreEqual(booking.EndTime, booking.StartTime.AddHours(2));
    }

    [TestMethod]
    public async Task PlaceBooking_WhenBookingIsCanceled_TableIsAvailableForNewBooking()
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
            Status = BookingStatus.Canceled,
            Guest = new Guest { 
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
        Assert.IsNull(error);

        var bookings = await ctx.Bookings.ToListAsync();
        // Check that booth bookings are registered:
        Assert.AreEqual(2, bookings.Count);
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
            Guest = new Guest { 
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