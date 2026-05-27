//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using Microsoft.Identity.Client;
//using Moq;
//using Restaurant.API.Controllers;
//using Restaurant.API.Data;
//using Restaurant.API.Services;
//using Restaurant.Models.Models;
//using Restaurant.Models.Models.Enums;

//namespace Restaurant.Test;

//[TestClass]
//public class EditBookingControllerTests
//{
//    private RestaurantDbContext CreateInMemoryDb()
//    {
//        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
//            .UseInMemoryDatabase(Guid.NewGuid().ToString())
//            .Options;
//        return new RestaurantDbContext(options);
//    }

//    [TestMethod]
//    public async Task CancelBooking_WhenBookingDoesNotExist_ReturnNotFound()
//    {
//        // Arrange
//        var ctx = CreateInMemoryDb();
//        var mockTableService = new Mock<ITableService>();
        
//        var controller = new EditBookingController(ctx, mockTableService.Object);

//        // Act
//        var result = await controller.CancelBooking(999);

//        // Assert
//        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
//    }

//    [TestMethod]
//    public async Task ConfirmBooking_WhenBookingDoesNotExist_ReturnNotFound()
//    {
//        // Arrange
//        var ctx = CreateInMemoryDb();
//        var mockTableService = new Mock<ITableService>();
//        var controller = new EditBookingController(ctx, mockTableService.Object);

//        // Act
//        var result = await controller.ConfirmBooking(999);

//        // Assert
//        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
//    }
//    [TestMethod]
//    public async Task CompleteBooking_WhenBookingDoesNotExist_ReturnNotFound()
//    {
//        var ctx = CreateInMemoryDb();
//        var mockTableService = new Mock<ITableService>();
//        //arrange
//        var controller = new EditBookingController(ctx, mockTableService.Object);
//        //act
//        var result = await controller.CompleteBooking(999);
//        //assert
//        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
//    }

//    [TestMethod]
//    public async Task CancelBooking_WhenBookingAlreadyCancelled_ReturnBadRequest()
//    {
//        var _ctx = CreateInMemoryDb();
//        var _controller = new EditBookingController(_ctx, new Mock<ITableService>().Object);
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 1,
//            Status = BookingStatus.Canceled
//        };
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();

//        //Act
//        var result = await _controller.CancelBooking(1);

//        //Assert
//        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//    }

//    [TestMethod]
//    public async Task ConfirmBooking_WhenBookingAlreadyConfirmed_ReturnBadRequest()
//    {
//        var _ctx = CreateInMemoryDb();
//        var _controller = new EditBookingController(_ctx, new Mock<ITableService>().Object);
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 1,
//            Status = BookingStatus.Confirmed
//        };
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();

//        //Act
//        var result = await _controller.ConfirmBooking(1);

//        //Assert
//        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//    }

//    [TestMethod]
//    public async Task CompleteBooking_WhenBookingAlreadyCompleted_ReturnBadRequest()
//    {
//        var _ctx = CreateInMemoryDb();
//        var _controller = new EditBookingController(_ctx, new Mock<ITableService>().Object);
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 1,
//            Status = BookingStatus.Complete
//        };
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();
//        //Act
//        var result = await _controller.CompleteBooking(1);
//        //Assert
//        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//    }

//    [TestMethod]
//    public async Task ConfirmBooking_WhenBookingIsPutToConfirmed_ReturnsOk()
//    {
//        var _ctx = CreateInMemoryDb();
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 99,
//            Status = BookingStatus.Canceled
//        };
//        booking.Status = BookingStatus.Confirmed;
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();
//        //Act
//        bool bookingIsConfirmed = false;
//        if (booking.Status == BookingStatus.Confirmed)
//        {
//            bookingIsConfirmed = true;
//        }
//        //Assert
//        Assert.IsTrue(bookingIsConfirmed);
//    }
//    [TestMethod]
//    public async Task CancelBooking_WhenBookingIsPutToCancelled_ReturnsOk()
//    {
//        var _ctx = CreateInMemoryDb();
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 99,
//            Status = BookingStatus.Confirmed
//        };
//        booking.Status = BookingStatus.Canceled;
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();
//        //Act
//        bool bookingIsCancelled = false;
//        if (booking.Status == BookingStatus.Canceled)
//        {
//            bookingIsCancelled = true;
//        }
//        //Assert
//        Assert.IsTrue(bookingIsCancelled);
//    }
//    [TestMethod]
//    public async Task CompleteBooking_WhenBookingIsPutToComplete_ReturnsOk()
//    {
//        var _ctx = CreateInMemoryDb();
//        //Arrange
//        var booking = new Booking
//        {
//            Id = 99,
//            Status = BookingStatus.Confirmed
//        };
//        booking.Status = BookingStatus.Complete;
//        _ctx.Bookings.Add(booking);
//        await _ctx.SaveChangesAsync();
//        //Act
//        bool bookingIsCompleted = false;
//        if (booking.Status == BookingStatus.Complete)
//        {
//            bookingIsCompleted = true;
//        }
//        //Assert
//        Assert.IsTrue(bookingIsCompleted);
//    }
//}
