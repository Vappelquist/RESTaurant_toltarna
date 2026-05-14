using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
using static REST_aurant.API.DTOs.Booking;
using System.Globalization;

namespace REST_aurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly RestaurantDbContext _ctx;

        public BookingController(RestaurantDbContext ctx)
        {
            _ctx = ctx;
        }
        //Get: AllBookings
        [HttpGet(Name = "GetAllBookings")]
        public async Task<ActionResult<IEnumerable<GetAllBookingResponse>>> GetAllBookings()
        {
            var bookings = await _ctx.Bookings
                .AsNoTracking()
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
            if(!bookings.Any())
            {
                return NotFound("No booking");
            }
         
            return Ok(bookings);
        }

        //Get: weekly bookings by week
        [HttpGet("GetWeeklyBookings")]
        public async Task<ActionResult<IEnumerable<GetAllBookingResponse>>> GetWeeklyBookings(int week)
        {
            //Get current year
            var year = DateTime.Now.Year;

            var startOfWeek = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
            //The week starts at Monday
            var endOfWeek = startOfWeek.AddDays(7);

            var bookings = await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.StartTime >= startOfWeek && b.StartTime < endOfWeek)
                .Select(b => new GetAllBookingResponse
                {
                    BookingId = b.Id,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    AmountOfGuests = b.AmountOfGuests,
                    Status = b.Status,
                    DateBooked = DateOnly.FromDateTime(b.DateBooked),
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();

            if(!bookings.Any())
            {
                return NotFound("No booking this week");
            }

            return Ok(bookings);
        }
    }
}
