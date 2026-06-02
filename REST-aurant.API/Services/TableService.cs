using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Services.Enums;
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

            var perfectTable = availableTables
                .Where(t => t.Seats >= amountOfGuests)
                .OrderBy(t => t.Seats)
                .FirstOrDefault();

            if (perfectTable != null)
                return new List<Table> { perfectTable };


            //if one table is not enough for booking, combine with biggest table first:
            var remainingTables = availableTables.OrderByDescending(t => t.Seats).ToList();
            var tablesToAssign = new List<Table>();
            int seatsNeeded = amountOfGuests;

            while (seatsNeeded > 0 && remainingTables.Any())
            {
                // Find smallest possible table to cover the rest of the guests:
                var bestFit = remainingTables
                    .Where(t => t.Seats >= seatsNeeded)
                    .OrderBy(t => t.Seats)
                    .FirstOrDefault();

                if (bestFit != null)
                {
                    tablesToAssign.Add(bestFit);
                    return tablesToAssign;
                }

                var largest = remainingTables.First();
                tablesToAssign.Add(largest);
                seatsNeeded -= largest.Seats;
                remainingTables.Remove(largest);

            }
            if (seatsNeeded > 0)
            {
                // If we do not have enough seats in the resturant, we'll return an enmpty list. 
                return new List<Table>();
            }

            return tablesToAssign;
        }

        public async Task<List<Table>> GetAllTablesAsync()
        {
            return await _ctx.Tables.OrderBy(t => t.TableNumber).ToListAsync();
        }

        public async Task<ServiceResult> AddTableAsync(int tableNumber, int seats)
        {
            var takenTable = await _ctx.Tables.AnyAsync(t => t.TableNumber == tableNumber);
            if (takenTable)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorType = ErrorType.TableAlreadyExists
                };
            }

            await _ctx.Tables.AddAsync(new Table
            {
                TableNumber = tableNumber,
                Seats = seats
            });
            await _ctx.SaveChangesAsync();
            return new ServiceResult
            {
                Success = true
            };
        }

        public async Task<ServiceResult> DeleteTableAsync(int tableNumber)
        {
            var table = await _ctx.Tables.FindAsync(tableNumber);
            if (table == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorType = ErrorType.TableNotFound
                };

            }
            var hasActiveBookings = await _ctx.Bookings
            .AnyAsync(b => b.Tables.Any(t => t.TableNumber == tableNumber)
            && b.Status != BookingStatus.Canceled
            && b.Status != BookingStatus.Complete);

            if (hasActiveBookings)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorType = ErrorType.TableHasActiceBookings
                };
            }

            _ctx.Tables.Remove(table);
            await _ctx.SaveChangesAsync();
            return new ServiceResult
            {
                Success = true
            };
        }

        public async Task<ServiceResult> EditTableAsync(int tableNumber, int seats)
        {
            var table = await _ctx.Tables.FindAsync(tableNumber);
            if (table == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorType = ErrorType.TableNotFound
                };
            }

            table.Seats = seats;
            await _ctx.SaveChangesAsync();
            return new ServiceResult
            {
                Success = true
            };
        }
    }
}