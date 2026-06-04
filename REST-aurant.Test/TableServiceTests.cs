using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Services;

namespace Restaurant.Test;

[TestClass]
public class TableServiceTests
{
    private RestaurantDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantDbContext(options);
    }
    [TestMethod]
    public async Task EditTableAsync_TryToEditExistingTable_ReturnsSuccess() 
    {

        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);

        ctx.Tables.Add(new Models.Models.Table
        {
            TableNumber = 99,
            Seats = 4
        });
        await ctx.SaveChangesAsync();
        // Act
        var result = await services.EditTableAsync(99, 6);

        // Assert
        Assert.IsPositive(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToEditNonExistingTable_ReturnsNotFound()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        // Act
        var result = await services.EditTableAsync(999, 6);
        // Assert
        Assert.IsFalse(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToAssignNegativeSeats_ReturnsBadRequest()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        ctx.Tables.Add(new Models.Models.Table
        {
            TableNumber = 99,
            Seats = 4
        });
        await ctx.SaveChangesAsync();
        // Act
        var result = await services.EditTableAsync(99, -1);
        // Assert
        Assert.IsTrue(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToAssignZeroSeats_ReturnsBadRequest()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        ctx.Tables.Add(new Models.Models.Table
        {
            TableNumber = 99,
            Seats = 4
        });
        await ctx.SaveChangesAsync();
        // Act
        var result = await services.EditTableAsync(99, 0);
        // Assert
        Assert.IsTrue(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToAssignTooManySeats_ReturnsBadRequest()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        ctx.Tables.Add(new Models.Models.Table
        {
            TableNumber = 99,
            Seats = 4
        });
        await ctx.SaveChangesAsync();
        // Act
        var result = await services.EditTableAsync(99, 100);
        // Assert
        Assert.IsTrue(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToAssignSeatsToNonExistingTable_ReturnsNotFound()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        // Act
        var result = await services.EditTableAsync(999, 4);
        // Assert
        Assert.IsFalse(result.Success);
    }


    [TestMethod]
    public async Task EditTableAsync_TryToAssignSeatsToExistingTable_ReturnsSuccess()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        ctx.Tables.Add(new Models.Models.Table
        {
            TableNumber = 99,
            Seats = 4
        });
        await ctx.SaveChangesAsync();
        // Act
        var result = await services.EditTableAsync(99, 8);
        // Assert
        Assert.IsTrue(result.Success);
    } 
}
