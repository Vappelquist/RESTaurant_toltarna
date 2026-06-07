using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;

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
    //-- EditTableAsync Tests --
    [TestMethod]
    public async Task EditTableAsync_TryToEditExistingTable_ReturnsSuccess()
    {

        // Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);

        ctx.Tables.Add(new Table
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
        ctx.Tables.Add(new Table
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
        ctx.Tables.Add(new Table
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
        ctx.Tables.Add(new Table
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
        ctx.Tables.Add(new Table
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


    // --GetAllTablesAsync Tests--
    [TestMethod]
    public async Task GetAllTablesAsync_WithMultipleTables_ReturnsAllTables()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);
        ctx.Tables.AddRange(
            new Table { TableNumber = 1, Seats = 4 },
            new Table { TableNumber = 2, Seats = 6 },
            new Table { TableNumber = 3, Seats = 2 }
            );
        await ctx.SaveChangesAsync();

        //act
        var result = await service.GetAllTablesAsync();

        //Assert
        Assert.HasCount(3, result);
        Assert.AreEqual(1, result[0].TableNumber);
        Assert.AreEqual(2, result[1].TableNumber);
        Assert.AreEqual(3, result[2].TableNumber);
    }

    [TestMethod]
    public async Task GetAllTablesAsync_WithNoTables_ReturnsEmptyList()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);

        //act
        var result = await service.GetAllTablesAsync();
        
        //Assert
        Assert.HasCount(0, result);
    }

    [TestMethod]
    public async Task GetAllTablesAsync_WithSingleTable_ReturnsListWithOneTable()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);
        ctx.Tables.Add(new Table { TableNumber = 1, Seats = 4 });
        await ctx.SaveChangesAsync();
        
        //act
        var result = await service.GetAllTablesAsync();
        
        //Assert
        Assert.HasCount(1, result);
    }
    [TestMethod]
    public async Task GetAllTablesAsync_WithMultipleTables_ReturnsDtoWithCorrectValues()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);
        ctx.Tables.AddRange(new Table { TableNumber = 5, Seats = 8 });
        await ctx.SaveChangesAsync();
        //act
        var result = await service.GetAllTablesAsync();
        //Assert
        Assert.AreEqual(5, result[0].TableNumber);
        Assert.AreEqual(8, result[0].Seats);
        
    }

    //--GetAvailavleTablesAsync Tests--
    [TestMethod]
    public async Task GetAvailableTablesAsync_WithNoOverlappings_ReturnsAllTables()
    {
        // Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);
        ctx.Tables.AddRange(
            new Table { TableNumber = 1, Seats = 4 },
            new Table { TableNumber = 2, Seats = 6 }
            );
        await ctx.SaveChangesAsync();
        var start = DateTime.Now;
        var end = DateTime.Now.AddHours(2);
        //act
        var result = await service.GetAvailableTablesAsync(start, end);
        //Assert
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public async Task GetAvailableTablesAsync_WithFullyOverlappingBooking_ReturnsOnlyUnbookedTables()
    {
        //Arrange
        using var ctx = CreateInMemoryDb();
        var service = new TableService(ctx);
        var table1 = new Table { TableNumber = 1, Seats = 4 };
        var table2 = new Table { TableNumber = 2, Seats = 6 };
        ctx.Tables.AddRange(table1, table2);

        var start = DateTime.Now;
        var end = DateTime.Now.AddHours(2);

        ctx.Bookings.Add(new Booking
        {
            StartTime = start,
            EndTime = end,
            Status = Models.Models.Enums.BookingStatus.Confirmed,
            Tables = new List<Table> { table1 }
        });
        await ctx.SaveChangesAsync();

        //Act
        var result = await service.GetAvailableTablesAsync(start, end);

        //Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(2, result[0].TableNumber);
    }
    [TestMethod]
    public async Task GetAvailableTablesAsync_WithCancelledBooking_ReturnsTableAsAvailable()
    {
        //Arrange
        using var ctx = CreateInMemoryDb();
        var services = new TableService(ctx);
        var table1 = new Table { TableNumber = 1, Seats = 4 };
        ctx.Tables.Add(table1);

        var start = DateTime.Now;
        var end = start.AddHours(2);

        ctx.Bookings.Add(new Booking
        {
            StartTime = start,
            EndTime = end,
            Status = BookingStatus.Canceled,
            Tables = new List<Table> { table1 }
        });
        await ctx.SaveChangesAsync();

        //Act
        var result = await services.GetAvailableTablesAsync(start, end);

        //Assert
        Assert.AreEqual(1, result.Count);
    } 
}
