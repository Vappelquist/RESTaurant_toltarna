using Microsoft.AspNetCore.Mvc;
using Restaurant.API.DTOs;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllTables()
        {
            var tables = await _tableService.GetAllTablesAsync();
            if (!tables.Any())
            {
                return NotFound("No tables found.");
            }
            return Ok(tables);
        }

        [HttpPost]
        public async Task<ActionResult> AddTable(AddTableRequest request)
        {
            var requestResult = await _tableService.AddTableAsync(request.TableNumber, request.Seats);
            if (!requestResult.Success)
            {
                return requestResult.ErrorType switch
                {
                    ErrorType.TableAlreadyExists => BadRequest("This tablenumber is already taken."),
                    _ => BadRequest()
                };
            }
            return Ok($"Table {request.TableNumber} was added.");
        }

        [HttpDelete("{tableNumber}")]
        public async Task<ActionResult> DeleteTable(int tableNumber)
        {
            var requestResult = await _tableService.DeleteTableAsync(tableNumber);
            if (!requestResult.Success)
            {
                return requestResult.ErrorType switch
                {
                    ErrorType.TableNotFound => NotFound($"Table {tableNumber} does not exist."),
                    ErrorType.TableHasActiceBookings => BadRequest($"Table {tableNumber} has active bookings."),
                    _ => BadRequest()
                };
            }
            return Ok($"Table {tableNumber} deleted successfully.");
        }

        [HttpPut("{tableNumber}")]
        public async Task<ActionResult> EditTable(int tableNumber, EditTableRequest request)
        {
            var requestResult = await _tableService.EditTableAsync(tableNumber, request.Seats);
            if (!requestResult.Success)
            {
                return requestResult.ErrorType switch
                {
                    ErrorType.TableNotFound => NotFound($"Table {tableNumber} does not exist."),
                    _ => BadRequest()
                };
            }
            return Ok($"Table {tableNumber} was updated.");
        }
    }
}
