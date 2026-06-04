using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.Test;

[TestClass]
public class GuestControllerTests
{
    [DataTestMethod]
    [DataRow("FirstName", "FirstName is reuquired")]
    [DataRow("LastName", "LastName is required")]
    [DataRow("Email", "Email is required")]
    [DataRow("Password", "Password is required")]
    public async Task AddGuest_WhenRequiredFieldIsMissing_ShouldReturnBadRequest(string field, string errorMessage)
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        var controller = new GuestController(null!, mockService.Object);
        controller.ModelState.AddModelError(field, errorMessage);

        var request = new CreateAddGuestRequest
        {
            FirstName = "Mini",
            LastName = "Momo",
            Email = "minimomo@mail.com",
            Password = "password123"
        };

        //Act
        var result = await controller.AddGuest(request);

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task AddGuest_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.AddGuestAsync(It.IsAny<CreateAddGuestRequest>()))
            .ReturnsAsync(new ServiceResult<Guest>
            {
                Success = false,
                ErrorType = ErrorType.ContactDetailsTaken
            });

        var controller = new GuestController(null!, mockService.Object);
        var request = new CreateAddGuestRequest
        {
            FirstName = "Mini",
            LastName = "Momo",
            Email = "minimomo@mail.com",
            Password = "password123"
        };

        //Act
        var result = await controller.AddGuest(request);

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetAllGuests_WhenGuestsExist_ShouldReturnOk()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();

        var controller = new GuestController(null!, mockService.Object);
        mockService.Setup(s => s.GetAllGuestsAsync())
            .ReturnsAsync(new List<GetGuestResponse>
        {
            new GetGuestResponse
            {   Id = 1,
                FirstName = "Anna",
                LastName = "Svensson",
                Email = "anna@mail.com"
            }
        });

        //Act
        var result = await controller.GetAllGuests();

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task AddGuest_WhenAllergiesAndNoteAreEmpty_ReturnOk()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();

        var request = new CreateAddGuestRequest
        {
            FirstName = "Mini",
            LastName = "Momo",
            Email = "minimomo@mail.com",
            Password = "password1234",
            Allergies = null,
            Note = null
        };
        mockService.Setup(s => s.AddGuestAsync(request))
            .ReturnsAsync(new ServiceResult<Guest>
            {
                Success = true,
                Data = new Guest()
            });
        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.AddGuest(request);

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
    [TestMethod]
    public async Task GetAllGuests_WhenNoGuestsExist_ShouldReturnNotFound()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        var controller = new GuestController(null!, mockService.Object);

        mockService.Setup(s => s.GetAllGuestsAsync())
            .ReturnsAsync(new List<GetGuestResponse>());

        //Act
        var result = await controller.GetAllGuests();

        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task DeleteGuest_WhenGuestDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.DeleteGuestByIdAsync(50))
            .ReturnsAsync(new ServiceResult
            {
                Success = false,
                ErrorType = ErrorType.GuestNotFound
            });
        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.DeleteGuest(50);

        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task DeleteGuest_WhenGuestExists_ShouldReturnOk()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.DeleteGuestByIdAsync(1))
            .ReturnsAsync(new ServiceResult
            {
                Success = true
            });

        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.DeleteGuest(1);

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetGuestByEmail_WhenEmailDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.GetGuestByEmailAsync("blabla@mail.com"))
            .ReturnsAsync((Guest?)null);

        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.GetGuestByEmail("blabla@mail.com");

        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetGuestByEmail_WhenEmailExists_ShouldReturnOk()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.GetGuestByEmailAsync("anna@mail.com"))
            .ReturnsAsync(new Guest
            {
                Id = 1,
                FirstName = "Anna",
                LastName = "Svensson",
                Email = "anna@mail.com"
            });

        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.GetGuestByEmail("anna@mail.com");

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task UpdateGuest_WhenUpdateGuestFails_ShouldReturnBadRequest()
    {
        //Arrange
        var mockService = new Mock<IGuestService>();
        mockService.Setup(s => s.UpdateGuestAsync(1, It.IsAny<UpdateGuestRequest>()))
            .ReturnsAsync(new ServiceResult<Guest>
            {
                Success = false,
                ErrorType = ErrorType.InvalidInput
            });

        var controller = new GuestController(null!, mockService.Object);

        //Act
        var result = await controller.UpdateGuest(1, new UpdateGuestRequest());

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
