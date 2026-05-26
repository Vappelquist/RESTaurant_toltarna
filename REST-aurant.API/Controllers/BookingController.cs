using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Services;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
using System.Globalization;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly RestaurantDbContext _ctx;
        private readonly IBookingService _bookingService;
        private readonly ITableService _tableService;

        public BookingController(RestaurantDbContext ctx, IBookingService bookingService, ITableService tableService)
        {
            _ctx = ctx;
            _bookingService = bookingService;
            _tableService = tableService;

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
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
            if (!bookings.Any())
            {
                return NotFound("No booking");
            }

            return Ok(bookings);
        }

        [HttpGet("{id}", Name = "GetBookingById")]
        public async Task<ActionResult> GetBookingById(int id)
        {
            var selectedBooking = await _ctx.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Tables)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (selectedBooking == null)
            {
                return NotFound();
            }
            else if (selectedBooking != null)
            {
                return Ok(new GetAllBookingResponse
                {
                    BookingId = selectedBooking.Id,
                    GuestName = $"{selectedBooking.Guest?.FirstName} {selectedBooking.Guest?.LastName}",
                    AmountOfGuests = selectedBooking.AmountOfGuests,
                    Status = selectedBooking.Status,
                    DateBooked = DateOnly.FromDateTime(selectedBooking.DateBooked),
                    StartDate = DateOnly.FromDateTime(selectedBooking.StartTime),
                    StartTime = TimeOnly.FromDateTime(selectedBooking.StartTime),
                    EndDate = DateOnly.FromDateTime(selectedBooking.EndTime),
                    EndTime = TimeOnly.FromDateTime(selectedBooking.EndTime),
                    BookingNotes = selectedBooking.BookingNotes,
                    TableNumbers = selectedBooking.Tables.Select(t => t.TableNumber).ToList()
                });
            }
            return BadRequest();

        }

        //Get: weekly bookings by week
        [HttpGet("GetWeeklyBookings")]
        public async Task<ActionResult<IEnumerable<GetAllBookingResponse>>> GetWeeklyBookings(int year, int week)
        {
            //Get current year
            var currentYear = DateTime.Now.Year;

            //current year and 2 years ahead
            if (year < currentYear || year > currentYear + 2)
            {
                return BadRequest($"Year must be between {currentYear} and {currentYear + 2}");
            }

            var totalWeeks = ISOWeek.GetWeeksInYear(year);

            if (week < 1 || week > totalWeeks)
            {
                return BadRequest($"Week must be between 1 and {totalWeeks} for year {year}");
            }
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
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.StartTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();

            if (!bookings.Any())
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

            int monthNumber;
            //Check if input is already a number 
            if (!int.TryParse(month, out monthNumber))
            {
                if (!DateTime.TryParse($"1 {month} 2026", out var parsedDate))
                {
                    return BadRequest("Type in a valid month or check the spelling.");
                }

                monthNumber = parsedDate.Month;
            }

            if (monthNumber < 1 || monthNumber > 12)
            {
                return BadRequest("Month number must be between 1 and 12.");
            }

            //Get current year
            var currentYear = DateTime.Now.Year;

            if (year < currentYear || year > currentYear + 2)
            {
                return BadRequest($"Year must be between {currentYear} and {currentYear + 2}");
            }

            //1st date of choosen month
            var startOfMonth = new DateTime(currentYear, monthNumber, 1);
            //End of choosen month ends the day right before 1st date the next month 
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
                    StartDate = DateOnly.FromDateTime(b.StartTime),
                    StartTime = TimeOnly.FromDateTime(b.EndTime),
                    EndDate = DateOnly.FromDateTime(b.EndTime),
                    EndTime = TimeOnly.FromDateTime(b.EndTime),
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



        //Endpoint to place a new booking
        [HttpPost("PlaceBooking")]
        public async Task<ActionResult> PlaceBooking(PlaceBookingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest("To place a booking you must enter your first name and last name.");
            }

            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest("You must provide either an email or phone number.");
            }

            bool isNumericString = request.PhoneNumber!.All(Char.IsDigit);
            if(!isNumericString)
            {
                return BadRequest("Phone number can only contain numbers.");
            }

            if (request.AmountOfGuests < 1)
            {
                return BadRequest("Amount of guests must be at least 1.");
            }

            if (!TimeOnly.TryParse(request.StartTime, out var startTime))
            {
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");
            }

            var error = await _bookingService.PlaceBookingAsync(request);
            if (error != null)
                return BadRequest(error);

            return Ok("Thank you, your booking has been received!");

        }

        [HttpGet("{id}/date", Name = "GetBookingDate")]
        public async Task<ActionResult<BookingDateDto>> GetBookingDate(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("No booking with this id");
            }
            var bookingDateDto = new BookingDateDto(DateOnly.FromDateTime(booking.DateBooked), $"{TimeOnly.FromDateTime(booking.StartTime).ToString("HH:mm")} - {TimeOnly.FromDateTime(booking.EndTime).ToString("HH:mm")}");
            return Ok(bookingDateDto);
        }

        // Get: View table availability by specific time
        [HttpGet("ViewBookingsByTime")]
        public async Task<ActionResult<IEnumerable<TableStatusDto>>> ViewBookingsByTime([FromQuery] DateOnly date, [FromQuery] string time)
        {
            // Validate the time input
            if (!TimeOnly.TryParse(time, out var startTime))
            {
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");
            }

            var startDateTime = date.ToDateTime(startTime);
            var endTime = startDateTime.AddHours(2); // Standard sittningstid på 2 timmar

            // Get all the tables in the resturant
            var allTables = await _ctx.Tables.ToListAsync();

            // Use TableService to get the list of available tables for the specified time
            var availableTables = await _tableService.GetAvailableTablesAsync(startDateTime, endTime);

            // Create a list of TableStatusDto to represent the status of each table
            var tableStatuses = allTables.Select(t => new TableStatusDto
            (
                t.TableNumber,
                t.Seats,
                availableTables.Any(at => at.TableNumber == t.TableNumber)
            )).ToList();

            return Ok(tableStatuses);
        }

    }
}
