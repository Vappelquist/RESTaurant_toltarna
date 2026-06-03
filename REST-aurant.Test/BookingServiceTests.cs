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
    /*
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
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Canceled });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CancelBookingAsync(1);

        //assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyCanceled, result.ErrorType);
    }

    [TestMethod]
    public async Task ConfirmBooking_WhenAlreadyConfirmed_ReturnsAlreadyConfirmed()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.ConfirmBookingAsync(1);

        //assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyConfirmed, result.ErrorType);
    }

    [TestMethod]
    public async Task CompleteBooking_WhenAlreadyComplete_ReturnsAlreadyComplete()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Complete });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CompleteBookingAsync(1);

        //assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyComplete, result.ErrorType);
    }

    [TestMethod]
    public async Task CancelBooking_WhenBookingIsConfirmed_StatusSetToCanceled()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CancelBookingAsync(1);

        //assert
        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Canceled, booking!.Status);
    }

    [TestMethod]
    public async Task ConfirmBooking_WhenBookingIsCanceled_StatusSetToConfirmed()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Canceled });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.ConfirmBookingAsync(1);

        //assert
        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Confirmed, booking!.Status);
    }

    [TestMethod]
    public async Task CompleteBooking_WhenBookingIsConfirmed_StatusSetToComplete()
    {
        //arrange
        var ctx = CreateInMemoryDb();
        ctx.Bookings.Add(new Booking { Id = 1, Status = BookingStatus.Confirmed });
        await ctx.SaveChangesAsync();

        //act
        var service = new BookingService(ctx, new Mock<ITableService>().Object);
        var result = await service.CompleteBookingAsync(1);

        //assert
        Assert.IsTrue(result.Success);
        var booking = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Complete, booking!.Status);
    }
    */
    // Edit booking tests: ------------------------------------------------------------------

    // GetWeeklyBookingsAsync-tests --------------------------------------------------------V

    [TestMethod]
    public async Task GetWeeklyBookingsAsync_WhenBookingsExistInWeek_ReturnsOnlyThoseBookings()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        // Adds guest and table cuz GetWeeklyBookingsAsync needs them
        var guest = new Guest { FirstName = "Test", LastName = "Testsson", Email = "test@test.se" };
        var table = new Table { TableNumber = 1, Seats = 4 };

        // Week 10 year 2026 starts on March 2nd.
        var bookingInWeek = new Booking
        {
            Guest = guest,
            Tables = new List<Table> { table },
            AmountOfGuests = 2,
            StartTime = new DateTime(2026, 3, 4, 18, 0, 0), // wensday week 10
            EndTime = new DateTime(2026, 3, 4, 20, 0, 0)
        };
        var bookingOutsideWeek = new Booking
        {
            Guest = guest,
            Tables = new List<Table> { table },
            AmountOfGuests = 2,
            StartTime = new DateTime(2026, 3, 11, 18, 0, 0), // wensday week 11
            EndTime = new DateTime(2026, 3, 11, 20, 0, 0)
        };

        ctx.Bookings.AddRange(bookingInWeek, bookingOutsideWeek);
        await ctx.SaveChangesAsync();

        //creates the service using the in-memory db and a mock TableService (which we dont use in this method)
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.GetWeeklyBookingsAsync(2026, 10);

        // Assert
        Assert.AreEqual(1, result.Count); // We expect only 1 booking in the result
        Assert.AreEqual(bookingInWeek.Id, result.First().BookingId); // And it should be the one from week 10.
    }

    [TestMethod]
    public async Task GetWeeklyBookingsAsync_WhenNoBookingsInWeek_ReturnsEmptyList()
    {
        // Arrange
        var ctx = CreateInMemoryDb(); // Empty database.
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.GetWeeklyBookingsAsync(2026, 10);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count); // The list should be empty
    }

    // GetWeeklyBookingsAsync-tests --------------------------------------------------------^

    // edit booking--------------------------------------------------------------------------

    [TestMethod]
    public async Task EditBookingStatusAsync_WhenBookingDoesNotExist_ReturnsBookingNotFound()
    {
        // Arrange
        var ctx = CreateInMemoryDb();
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.EditBookingStatusAsync(999, "Confirmed");

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.BookingNotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task EditBookingStatusAsync_WhenInvalidStatus_ReturnsInvalidInput()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        ctx.Bookings.Add(new Booking
        {
            Id = 1,
            Status = BookingStatus.Pending,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        });

        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.EditBookingStatusAsync(1, "NotAStatus");

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.InvalidInput, result.ErrorType);
    }

    [TestMethod]
    public async Task EditBookingStatusAsync_WhenAlreadyConfirmed_ReturnsAlreadyConfirmed()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        ctx.Bookings.Add(new Booking
        {
            Id = 1,
            Status = BookingStatus.Confirmed,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        });

        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.EditBookingStatusAsync(1, "Confirmed");

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.AlreadyConfirmed, result.ErrorType);
    }

    [TestMethod]
    public async Task EditBookingStatusAsync_WhenValidStatus_UpdatesStatus()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        ctx.Bookings.Add(new Booking
        {
            Id = 1,
            Status = BookingStatus.Pending,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        });

        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.EditBookingStatusAsync(1, "Confirmed");

        // Assert
        Assert.IsTrue(result.Success);

        var updated = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Confirmed, updated!.Status);
    }

    [TestMethod]
    public async Task EditBookingStatusAsync_IsCaseInsensitive()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        ctx.Bookings.Add(new Booking
        {
            Id = 1,
            Status = BookingStatus.Pending,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        });

        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        // Act
        var result = await service.EditBookingStatusAsync(1, "cOnFiRmEd");

        // Assert
        Assert.IsTrue(result.Success);

        var updated = await ctx.Bookings.FindAsync(1);
        Assert.AreEqual(BookingStatus.Confirmed, updated!.Status);
    }


    [TestMethod]
    public async Task UpdateBookingDetailsAsync_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var ctx = CreateInMemoryDb();
        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        var request = new UpdateBookingDetailsRequest
        {
            FirstName = "Test"
        };

        // Act
        var result = await service.UpdateBookingDetailsAsync(999, request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.BookingNotFound, result.ErrorType);
    }

    [TestMethod]
    public async Task UpdateBookingDetailsAsync_DoesNotOverwriteWithEmptyValues()
    {
        // Arrange
        var ctx = CreateInMemoryDb();

        var booking = new Booking
        {
            Id = 1,
            Guest = new Guest
            {
                FirstName = "KeepFirst",
                LastName = "KeepLast",
                Email = "doe@mail.com"
            },
            BookingNotes = "Keep notes",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2)
        };

        ctx.Bookings.Add(booking);
        await ctx.SaveChangesAsync();

        var service = new BookingService(ctx, new Mock<ITableService>().Object);

        var request = new UpdateBookingDetailsRequest
        {
            FirstName = "   ",   // whitespace
            LastName = null,
            BookingNotes = null
        };

        // Act
        var result = await service.UpdateBookingDetailsAsync(1, request);

        // Assert
        Assert.IsTrue(result.Success);

        var updated = await ctx.Bookings.Include(b => b.Guest).FirstOrDefaultAsync(b => b.Id == 1);

        Assert.AreEqual("KeepFirst", updated!.Guest!.FirstName);
        Assert.AreEqual("KeepLast", updated.Guest!.LastName);
        Assert.AreEqual("Keep notes", updated.BookingNotes);
    }
    // edit booking--------------------------------------------------------------------------
}
