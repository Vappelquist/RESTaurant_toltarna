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
            PhoneNumber = "0701234567",
            AmountOfGuests = 2,
            StartTime = "18:00",
            BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var restult = await service.PlaceBookingAsync(request);

        // Assert
        var booking = await ctx.Bookings.FirstOrDefaultAsync();
        Assert.IsNotNull(booking);
        Assert.AreEqual(booking.StartTime.AddHours(2), booking.EndTime);
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

    // Edit booking tests: ------------------------------------------------------------------

    [TestMethod]
    public async Task CancelBooking_WhenBookingDoesNotExist_ReturnNotFound()
    {
        // Arrange
        var ctx = CreateInMemoryDb();
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.CancelBookingAsync(999);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.BookingNotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task ConfirmBooking_WhenBookingDoesNotExist_ReturnNotFound()
    {
        // Arrange
        var ctx = CreateInMemoryDb();
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.ConfirmBookingAsync(999);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.BookingNotFound, result.ErrorType);
    }
    [TestMethod]
    public async Task CompleteBooking_WhenBookingDoesNotExist_ReturnNotFound()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        //act
        var result = await service.CompleteBookingAsync(999);

        //assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.BookingNotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task CancelBooking_WhenAlreadyCanceled_ReturnsAlreadyCanceled()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Canceled });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CancelBookingAsync(1);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyCanceled, result.ErrorType);
    }

    [TestMethod]
    public async Task ConfirmBooking_WhenAlreadyConfirmed_ReturnsAlreadyConfirmed()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.ConfirmBookingAsync(1);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyConfirmed, result.ErrorType);
    }

    [TestMethod]
    public async Task CompleteBooking_WhenAlreadyComplete_ReturnsAlreadyComplete()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Complete });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CompleteBookingAsync(1);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyComplete, result.ErrorType);
    }

    [TestMethod]
    public async Task CancelBooking_WhenBookingIsConfirmed_StatusSetToCanceled()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CancelBookingAsync(1);

        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Canceled, booking!.Status);
    }

    [TestMethod]
    public async Task ConfirmBooking_WhenBookingIsCanceled_StatusSetToConfirmed()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Canceled });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.ConfirmBookingAsync(1);

        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Confirmed, booking!.Status);
    }

    [TestMethod]
    public async Task CompleteBooking_WhenBookingIsConfirmed_StatusSetToComplete()
    {
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CompleteBookingAsync(1);

        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Complete, booking!.Status);
    }
    // Edit booking tests: ------------------------------------------------------------------
}
