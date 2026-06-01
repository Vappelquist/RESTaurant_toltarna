using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Services;

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

    }
}
