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

        //Get: MonthlyBookings
        [HttpGet("GetMonthlyBookings")]
        public async Task<ActionResult<IEnumerable<GetAllBookingResponse>>> GetMonthlyBookings(int year, string month)
        {
            //Check if month is empty
            if (string.IsNullOrWhiteSpace(month))
            {
                return BadRequest("Month is required.");
            }
            //Check if year is invalid
            if (year <= 0)
            {
                return BadRequest("Type in a valid year.");
            }
            int monthNumber; 
            //Check if input is already a number 
            if (!int.TryParse(month, out monthNumber))
            {
                if(!DateTime.TryParse($"1 {month} 2026", out var parsedDate))
                {
                    return BadRequest("Type in a valid month or check the spelling.");
                }

                monthNumber = parsedDate.Month;
            }

            if(monthNumber <1 || monthNumber >12)
            {
                return BadRequest("Month number must be between 1 and 12.");
            }

            //Get current year
            var currentYear = DateTime.Now.Year;

            if(year < currentYear || year > currentYear +2 )
            {
                return BadRequest($"Year must be between {currentYear} and {currentYear + 2}");
            }

            //1 choosen month
            var startOfMonth = new DateTime(currentYear, monthNumber, 1);
            //End of choosen month ends the day right before 1st the next month 
            var endOfMonth = startOfMonth.AddMonths(1);

            var bookings = await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.StartTime >= startOfMonth && b.StartTime < endOfMonth)
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

            if (!bookings.Any())
            {
                return NotFound("No booking this month");
            }

            return Ok(bookings);
        }
    }
    
}
