using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
using REST_aurant.API.DTOs;
using Restaurant.Models.Models;
using Restaurant.Models.Models.Enums;
using System.Globalization;
using static REST_aurant.API.DTOs.Booking;

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

            if (request.AmountOfGuests < 1)
            {
                return BadRequest("Amount of guests must be at least 1.");
            }

            if (!TimeOnly.TryParse(request.StartTime, out var startTime))
            {
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");
            }

            var startDateTime = request.BookingDate.ToDateTime(startTime);

            //2 hours session
            var endTime = startDateTime.AddHours(2);

            var guest = await _ctx.Guests
                .FirstOrDefaultAsync(g => g.Email == request.Email || g.PhoneNumber == request.PhoneNumber);

            if (guest != null)
            {
                if (guest.FirstName != request.FirstName || guest.LastName != request.LastName)
                {
                    return BadRequest("This email or phone number already belongs to another guest.");
                }
            }

            if (guest == null)
            {
                guest = new Guest
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                };
                await _ctx.Guests.AddAsync(guest);
            }

            //Search for existing guest
            var unavailableTableNumbers = await _ctx.Bookings
                .AsNoTracking()
                .Where(b => b.Status != BookingStatus.Canceled)
                .Where(b => b.StartTime < endTime && b.EndTime > startDateTime)
                .SelectMany(b => b.Tables)
                .Select(t => t.TableNumber)
                .ToListAsync();

            var availableTables = await _ctx.Tables
                .Where(t => !unavailableTableNumbers.Contains(t.TableNumber))
                .ToListAsync();

            var seats = availableTables.Sum(t => t.Seats);
            if(seats < request.AmountOfGuests)
            {
                return BadRequest("This requested time is fully booked, please choose another available time.");
            }

            var placeBooking = new Restaurant.Models.Models.Booking
            {
                Guest = guest,
                AmountOfGuests = request.AmountOfGuests,
                DateBooked = DateTime.Now,
                StartTime = startDateTime,
                EndTime = endTime,
                Status = BookingStatus.Confirmed,
                BookingNotes = request.BookingNotes
            };
            await _ctx.Bookings.AddAsync(placeBooking);
            await _ctx.SaveChangesAsync();
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
    }
}
