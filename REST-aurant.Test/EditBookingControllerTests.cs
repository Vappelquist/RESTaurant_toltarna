using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;

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
}
