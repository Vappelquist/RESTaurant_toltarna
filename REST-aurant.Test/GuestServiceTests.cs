using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.Models.Models;
using Restaurant.API.Services.Enums;
using System.ComponentModel.DataAnnotations;
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

    [TestMethod]
    public async Task AddGuestAsync_WhenValid_ReturnSuccess()
    {
        // Arrange
        var ctx = CreateInMemoryDb();
        var service = new GuestService(ctx);
        var request = new CreateAddGuestRequest
        {
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@mail.com",
            Password = "password123"
        };

        // Act
        var result = await service.AddGuestAsync(request);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [DataTestMethod]
    [DataRow(null, "PhoneNumber is null, should still be accepted since it's not required")]
    [DataRow("", "PhoneNumber is empty, should still be accepted since it's not required")]
    public async Task AddGuestAsync_WhenPhoneNumberIsNullOrEmpty_ShouldSucceed(string phoneNumber, string errorMessage)
    {
        //Arrange
        var ctx = CreateInMemoryDb();
        var service = new GuestService(ctx);

        var request = new CreateAddGuestRequest
        {
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@mail.com",
            Password = "password123",
            PhoneNumber = phoneNumber
        };

        //Act
        var result = await service.AddGuestAsync(request);

        //Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task AddGuestAsync_WhenEmailAlreadyExists_ReturnContactDetailsTaken()
    {
        //Arrange
        var ctx = CreateInMemoryDb();
        var service = new GuestService(ctx);

        ctx.Guests.Add(new Guest
        {
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@mail.com",
            Password = "password123"
        });

        await ctx.SaveChangesAsync();

        var request = new CreateAddGuestRequest
        {
            FirstName = "Mini",
            LastName = "Momo",
            Email = "anna@mail.com",
            Password = "password456"
        };

        //Act
        var result = await service.AddGuestAsync(request);

        //Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ErrorType.ContactDetailsTaken, result.ErrorType);
    }

    [TestMethod]
    public async Task GetAllGuestsAsync_WhenGuestsExist_ShouldReturnAllGuests()
    {
        //Arrange
        var ctx = CreateInMemoryDb();
        var service = new GuestService(ctx);

        await ctx.Guests.AddAsync(new Guest
        {
            FirstName = "Anna",
            LastName = "Svensson",
            Email = "anna@mail.com",
            Password = "password123"
        });

        await ctx.Guests.AddAsync(new Guest
        {
            FirstName = "Mini",
            LastName = "Momo",
            Email = "minimomo@mail.com",
            Password = "password456"
        });

        await ctx.SaveChangesAsync();

        //Act
        var result = await service.GetAllGuestsAsync();

        //Assert
        Assert.AreEqual(2, result.Count);
    }
}
