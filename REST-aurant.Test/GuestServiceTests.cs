using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.Models.Models;
using Restaurant.API.Services.Enums;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.Test;

[TestClass]
public class GuestServiceTests
{
    // For in-memory-database:
    private RestaurantDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantDbContext(options);
    }

    [DataTestMethod]
    [DataRow("", "Svensson", "FirstName cannot be empty")]
    [DataRow("Anna", "", "LastName cannot be empty")]
    [DataRow("", "", "both FirstName and LastName cannot be empty")]
    [DataRow(null, "Svensson", "FirstName cannot be null")]
    [DataRow("Anna", null, "LastName cannot be null")]
    [DataRow("   ", "Svensson", "FirstName containing whitespace should not pass")]
    [DataRow("Anna", "   ", "LastName containing whitespace should not pass")]
    public async Task AddGuest_WhenNameIsMissing_ShouldReturnInvaldInputErrorType(string firstName, string lastName, string errorMessage)
    {
        //Arrange
        var ctx = CreateInMemoryDb();
        var service = new GuestService(ctx);

        var request = new CreateAddGuestRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = "anna@mail.com",
            Password = "password123",
            PhoneNumber = "0701234567"
        };
        //Act
        var result = await service.AddGuestAsync(request);

        //Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.InvalidInput, result.ErrorType);
    }
}
