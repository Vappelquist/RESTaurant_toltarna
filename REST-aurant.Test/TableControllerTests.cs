using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.API.Controllers;
using Restaurant.API.DTOs;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;

namespace Restaurant.Test
{
    [TestClass]
    public class TableControllerTests
    {
        // GetAllTables-tests ---------------------------------------------------------------V
        [TestMethod]
        public async Task GetAllTables_WhenTablesExist_ReturnsOk()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();

            // Bytte ut Table mot TableDto här:
            mockTableService.Setup(s => s.GetAllTablesAsync())
                .ReturnsAsync(new List<TableDto> { new TableDto { TableNumber = 1, Seats = 4 } });

            var controller = new TableController(mockTableService.Object);

            // Act
            var result = await controller.GetAllTables();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetAllTables_WhenNoTablesExist_ReturnsNotFound()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();

            // Bytte ut Table mot TableDto här också:
            mockTableService.Setup(s => s.GetAllTablesAsync())
                .ReturnsAsync(new List<TableDto>());

            var controller = new TableController(mockTableService.Object);

            // Act
            var result = await controller.GetAllTables();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        // AddTable-tests ---------------------------------------------------------------V
        [TestMethod]
        public async Task AddTable_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.AddTableAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = true });

            var controller = new TableController(mockTableService.Object);
            var request = new AddTableRequest { TableNumber = 1, Seats = 4 };

            // Act
            var result = await controller.AddTable(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task AddTable_WhenTableAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.AddTableAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = false, ErrorType = ErrorType.TableAlreadyExists });

            var controller = new TableController(mockTableService.Object);
            var request = new AddTableRequest { TableNumber = 1, Seats = 4 };

            // Act
            var result = await controller.AddTable(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        // DeleteTable-tests ---------------------------------------------------------------V
        [TestMethod]
        public async Task DeleteTable_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.DeleteTableAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = true });

            var controller = new TableController(mockTableService.Object);

            // Act
            var result = await controller.DeleteTable(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteTable_WhenTableHasActiveBookings_ReturnsBadRequest()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.DeleteTableAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = false, ErrorType = ErrorType.TableHasActiveBookings });

            var controller = new TableController(mockTableService.Object);

            // Act
            var result = await controller.DeleteTable(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteTable_WhenTableNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.DeleteTableAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = false, ErrorType = ErrorType.TableNotFound });

            var controller = new TableController(mockTableService.Object);

            // Act
            var result = await controller.DeleteTable(99);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        // EditTable-tests ---------------------------------------------------------------V
        [TestMethod]
        public async Task EditTable_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.EditTableAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = true });

            var controller = new TableController(mockTableService.Object);
            var request = new EditTableRequest { Seats = 6 };

            // Act
            var result = await controller.EditTable(1, request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task EditTable_WhenTableNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockTableService = new Mock<ITableService>();
            mockTableService.Setup(s => s.EditTableAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ServiceResult { Success = false, ErrorType = ErrorType.TableNotFound });

            var controller = new TableController(mockTableService.Object);
            var request = new EditTableRequest { Seats = 6 };

            // Act
            var result = await controller.EditTable(99, request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}