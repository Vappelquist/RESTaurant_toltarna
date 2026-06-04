using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Services;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.Test;

[TestClass]
public class BookingControllerTests
{
    // PlaceBooking-tests ---------------------------------------------------------------V

    [TestMethod]
    public async Task PlaceBooking_WhenBookingSucceeds_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<IBookingService>();

        mockService
            .Setup(x => x.PlaceBookingAsync(It.IsAny<PlaceBookingRequest>()))
            .ReturnsAsync(new ServiceResult
            {
                Success = true,
                BookingId = 123
            });

        var controller = new BookingController(mockService.Object);

        // Act
        var result = await controller.PlaceBooking(new PlaceBookingRequest());

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

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
    [DataRow("auGUsTi")]
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

    // GetWeeklyBookings-tests ---------------------------------------------------------------V

    [TestMethod]
    [DataRow(2010)] // Too old
    [DataRow(2029)] // To far in the future
    public async Task GetWeeklyBookings_WhenYearIsInvalid_ReturnBadRequest(int invalidYear)
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var controller = new BookingController(mockBookingService.Object);

        // Act
        var result = await controller.GetWeeklyBookings(invalidYear, 10);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    [DataRow(0)]  // Weeks start at 1
    [DataRow(54)] // Only 52 weeks in a year
    public async Task GetWeeklyBookings_WhenWeekIsInvalid_ReturnBadRequest(int invalidWeek)
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var controller = new BookingController(mockBookingService.Object);
        var validYear = DateTime.Now.Year;

        // Act
        var result = await controller.GetWeeklyBookings(validYear, invalidWeek);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetWeeklyBookings_WhenNoBookingsExist_ReturnNotFound()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var validYear = DateTime.Now.Year;
        var validWeek = 10;

        // Force the fuck to return an empty list. 
        mockBookingService
            .Setup(s => s.GetWeeklyBookingsAsync(validYear, validWeek))
            .ReturnsAsync(new List<GetAllBookingResponse>());

        var controller = new BookingController(mockBookingService.Object);

        // Act
        var result = await controller.GetWeeklyBookings(validYear, validWeek);

        // Assert
        // The controller should return a NotFound result (404)
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    // GetWeeklyBookings-tests ---------------------------------------------------------------^

}