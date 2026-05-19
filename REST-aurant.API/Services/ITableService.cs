using Restaurant.Models.Models;

namespace REST_aurant.API.Services
{
    public interface ITableService
    {
        Task<List<Table>> GetAvailableTablesAsync(DateTime startDateTime, DateTime endTime, int? ignoreBookingId = null);
        Task<List<Table>> AllocateTablesAsync(DateTime startDateTime, DateTime endTime, int amountOfGuests, int? ignoreBookingId = null);
    }
}