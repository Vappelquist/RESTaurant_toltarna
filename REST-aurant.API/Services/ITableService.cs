using Restaurant.Models.Models;

namespace Restaurant.API.Services
{
    public interface ITableService
    {
        Task<List<Table>> GetAvailableTablesAsync(DateTime startDateTime, DateTime endTime, int? ignoreBookingId = null);
        Task<List<Table>> AllocateTablesAsync(DateTime startDateTime, DateTime endTime, int amountOfGuests, int? ignoreBookingId = null);
        Task<List<Table>> GetAllTablesAsync();
        Task<ServiceResult> AddTableAsync(int tableNumber, int seats);


    }
}