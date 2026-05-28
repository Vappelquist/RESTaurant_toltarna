using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Services;
using System.ComponentModel.DataAnnotations;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.Test;

[TestClass]
public class BookingControllerTests
{
    // PlaceBooking-tests ---------------------------------------------------------------V
    //[TestMethod]
    //public async Task PlaceBooking_WhenFirstNameIsMissing_ReturnBadRequest()
    //{
    //    // Arrange
    //    var mockBookingService = new Mock<IBookingService>();
    //    var controller = new BookingController(mockBookingService.Object); 

    //    var request = new PlaceBookingRequest
    //    {
    //        FirstName = "",
    //        LastName = "Svensson",
    //        Email = "sven@mail.com",
    //        AmountOfGuests = 2,
    //        StartTime = "18:00",
    //        BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    //    };

    //    // Act
    //    var result = await controller.PlaceBooking(request);

    //    // Assert
    //    Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    //}

    //[TestMethod]
    //public async Task PlaceBooking_WhenLastNameIsMissing_ReturnBadRequest()
    //{
    //    // Arrange
    //    var mockBookingService = new Mock<IBookingService>();
    //    var controller = new BookingController(mockBookingService.Object);

    //    var request = new PlaceBookingRequest
    //    {
    //        FirstName = "Sven",
    //        LastName = "",
    //        Email = "sven@mail.com",
    //        AmountOfGuests = 2,
    //        StartTime = "18:00",
    //        BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    //    };

    //    // Act
    //    var result = await controller.PlaceBooking(request);

    //    // Assert
    //    Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    //}

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

    //[TestMethod]
    //public async Task PlaceBooking_WhenAmountOfGuestsIsMissing_ReturnBadRequest()
    //{
    //    // Arrange
    //    var mockBookingService = new Mock<IBookingService>();
    //    var controller = new BookingController(mockBookingService.Object);

    //    var request = new PlaceBookingRequest
    //    {
    //        FirstName = "Sven",
    //        LastName = "Svensson",
    //        Email = "sven@mail.com",
    //        PhoneNumber = "+4676-5487999",
    //        AmountOfGuests = 0,
    //        StartTime = "18:00",
    //        BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    //    };

    //    // Act
    //    var result = await controller.PlaceBooking(request);

    //    // Assert
    //    Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    //}

    // PlaceBooking-tests ---------------------------------------------------------------^



    // GetMonthlyBookings-tests ---------------------------------------------------------------V

    [TestMethod]
    [DataRow("")]
    [DataRow("Muni")]
    [DataRow("0")]
    [DataRow("13")]
    [DataRow("-5")]
    [DataRow("99")]
    public async Task GetMonthlyBookings_WhenMonthIsInvalid_ReturnBadRequest(string month)
    {
        //arrange
        var mockBookingService = new Mock<IBookingService>();
        var controller = new BookingController(mockBookingService.Object);

        //act
        var result = await controller.GetMonthlyBookings(2026, month);

        //assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("12")]
    [DataRow("september")]
    public async Task GetMonthlyBookings_WhenMonthIsValid_ReturnOk(string month)
    {
        //arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService
            .Setup(s => s.GetMonthlyBookingsAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<GetAllBookingResponse>
            {
                new GetAllBookingResponse()
            });

        var controller = new BookingController(mockBookingService.Object);

        //act
        var result = await controller.GetMonthlyBookings(2026, month);

        //assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    // GetMonthlyBookings-tests ---------------------------------------------------------------^



    // GetWeeklyBookings-tests ---------------------------------------------------------------V

    // GetWeeklyBookings-tests ---------------------------------------------------------------^


}