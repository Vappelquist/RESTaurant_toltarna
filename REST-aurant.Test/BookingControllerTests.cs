using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
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

    [TestMethod]
    public async Task PlaceBooking_WhenContactDetailsTaken_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IBookingService>();

        mockService
            .Setup(x => x.PlaceBookingAsync(It.IsAny<PlaceBookingRequest>()))
            .ReturnsAsync(new ServiceResult
            {
                Success = false,
                ErrorType = ErrorType.ContactDetailsTaken
            });

        var controller = new BookingController(mockService.Object);

        // Act
        var result = await controller.PlaceBooking(new PlaceBookingRequest());

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            "This email or phone number already belongs to another guest.",
            badRequest.Value);
    }

    [TestMethod]
    public async Task PlaceBooking_WhenRestaurantIsFull_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IBookingService>();

        mockService
            .Setup(x => x.PlaceBookingAsync(It.IsAny<PlaceBookingRequest>()))
            .ReturnsAsync(new ServiceResult
            {
                Success = false,
                ErrorType = ErrorType.FullyBooked
            });

        var controller = new BookingController(mockService.Object);

        // Act
        var result = await controller.PlaceBooking(new PlaceBookingRequest());

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            "This time is fully booked, please choose another time.",
            badRequest.Value);
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


    //GetAllBookings-Test -----------------
    [TestMethod]
    public async Task GetAllBookings_WithExistingBookings_ReturnOk()
    {
        //Arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService
            .Setup(s => s.GetAllBookingsAsync())
            .ReturnsAsync(new List<GetAllBookingResponse>
            {
                new GetAllBookingResponse()
            });
        var controller = new BookingController(mockBookingService.Object);

        //Act
        var result = await controller.GetAllBookings();

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetAllBooking_WithNoBookings_ReturnsNotFound()
    {
        //Arrange
        var mockBuildingService = new Mock<IBookingService>();
        mockBuildingService
            .Setup(s => s.GetAllBookingsAsync())
            .ReturnsAsync(new List<GetAllBookingResponse>());
        var controller = new BookingController(mockBuildingService.Object);

        //Act
        var result = await controller.GetAllBookings();

        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

    }
    //GetBookingsById ----------------
    [TestMethod]
    public async Task GetBookingById_SearchWithExistingId_ReturnsOk()
    {
        //Arrange
        var mockBuildingService = new Mock<IBookingService>();
        mockBuildingService
        .Setup(s => s.GetBookingByIdAsync(It.IsAny<int>()))
        .ReturnsAsync(new GetAllBookingResponse());
        var controller = new BookingController(mockBuildingService.Object);
        //Act
        var result = await controller.GetBookingById(1);

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

    }
    [TestMethod]
    public async Task GetBookingById_SearchWithNonExistingId_ReturnsOk()
    {
        //Arrange
        var mockBuildingService = new Mock<IBookingService>();
        mockBuildingService
            .Setup(s => s.GetBookingByIdAsync(It.IsAny<int>()))
        .ReturnsAsync((GetAllBookingResponse?)null);
        var controller = new BookingController(mockBuildingService.Object);
        //Act
        var result = await controller.GetBookingById(99);

        //Arrange
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    //GetDailyBookings tests ------------------------
    [TestMethod]
    public async Task GetDailyBookings_SearchWithExistingDate_ReturnsOk()
    {
        //Arrange
        var mockBuildingService = new Mock<IBookingService>();
        mockBuildingService
        .Setup(s => s.GetDailyBookingAsync(It.IsAny<DateOnly>()))
            .ReturnsAsync(new List<GetAllBookingResponse>
                {
                    new GetAllBookingResponse()
                });
        var controller = new BookingController(mockBuildingService.Object);

        //Act
        var result = await controller.GetDailyBookings(new DateOnly(2026, 07, 11));
        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
    [TestMethod]
    public async Task GetDailyBookings_WithNoBookingsOnDate_ReturnsNotFound()
    {
        //Arrange
        var mockBuildingService = new Mock<IBookingService>();
        mockBuildingService
    .Setup(s => s.GetDailyBookingAsync(It.IsAny<DateOnly>()))
    .ReturnsAsync(new List<GetAllBookingResponse>());
        var controller = new BookingController(mockBuildingService.Object);

        //Act
        var result = await controller.GetDailyBookings(new DateOnly(2027, 07, 11));
        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }
}