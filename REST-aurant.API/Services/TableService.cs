using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;

namespace Restaurant.API.Services
{
    public class TableService : ITableService
    {
        private readonly RestaurantDbContext _ctx;

        public TableService(RestaurantDbContext ctx)
        {
            _ctx = ctx;
        }

        // Returns a list of tables that are available for booking between the given start and end time. 
        // Optionally, a booking id can be provided to ignore that booking when checking for availability (useful when updating an existing booking so it does not count itself as unavailable).
        public async Task<List<Table>> GetAvailableTablesAsync(DateTime startDateTime, DateTime endTime, int? ignoreBookingId = null)
        {
            var query = _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.Status != BookingStatus.Canceled)
                .Where(b => b.StartTime < endTime && b.EndTime > startDateTime);

            // If we added an id to ignore... ignore it. 
            if (ignoreBookingId.HasValue)
            {
                query = query.Where(b => b.Id != ignoreBookingId.Value);
            }

            var unavailableTableNumbers = await query
                .SelectMany(b => b.Tables)
                .Select(t => t.TableNumber)
                .ToListAsync();

            return await _ctx.Tables
                .Where(t => !unavailableTableNumbers.Contains(t.TableNumber))
                .ToListAsync();
        }

        //returns a list of tables that can be allocated to a booking based on the amount of guests.
        public async Task<List<Table>> AllocateTablesAsync(DateTime startDateTime, DateTime endTime, int amountOfGuests, int? ignoreBookingId = null)
        {
            var availableTables = await GetAvailableTablesAsync(startDateTime, endTime, ignoreBookingId);
            var tablesToAssign = new List<Table>();
            int seatsAllocated = 0;

            foreach (var table in availableTables)
            {
                tablesToAssign.Add(table);
                seatsAllocated += table.Seats;

                if (seatsAllocated >= amountOfGuests)
                {
                    return tablesToAssign;
                }
            }

            // If we do not have enough seats in the resturant, we'll return an enmpty list. 
            return new List<Table>();
        }
    }
}