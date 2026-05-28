using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.Test;

[TestClass]
public class BookingServiceTests
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
        var result = await bookingService.PlaceBookingAsync(newBooking);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.FullyBooked, result.ErrorType);
    }
}
