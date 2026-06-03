using Microsoft.AspNetCore.Mvc;
using Restaurant.API.DTOs;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
using System.Globalization;
using static Restaurant.API.DTOs.Booking;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet(Name = "GetAllBookings")]
        public async Task<ActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            if (!bookings.Any())
                return NotFound("No booking");
            return Ok(bookings);
        }

        [HttpGet("{id}", Name = "GetBookingById")]
        public async Task<ActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();
            return Ok(booking);
        }

        [HttpGet("GetDailyBookings")]
        [EndpointSummary("ADMIN Get daily bookings")]
        public async Task<ActionResult> GetDailyBookings(DateOnly date)
        {
            var bookings = await _bookingService.GetDailyBookingAsync(date);
            if (!bookings.Any())
            {
                return NotFound("No bookings found for this date.");

            }
            return Ok(bookings);
        }

        [HttpGet("GetWeeklyBookings")]
        public async Task<ActionResult> GetWeeklyBookings(int year, int week)
        {
            var currentYear = DateTime.Now.Year;
            if (year < currentYear || year > currentYear + 2)
                return BadRequest($"Year must be between {currentYear} and {currentYear + 2}");

            var totalWeeks = ISOWeek.GetWeeksInYear(year);
            if (week < 1 || week > totalWeeks)
                return BadRequest($"Week must be between 1 and {totalWeeks} for year {year}");

            var bookings = await _bookingService.GetWeeklyBookingsAsync(year, week);
            if (!bookings.Any())
                return NotFound("No booking this week");
            return Ok(bookings);
        }

        [HttpGet("GetMonthlyBookings")]
        public async Task<ActionResult> GetMonthlyBookings(int year, string month)
        {
            if (string.IsNullOrWhiteSpace(month))
                return BadRequest("Month is required.");

            int monthNumber;
            if (!int.TryParse(month, out monthNumber))
            {
                if (!DateTime.TryParse($"1 {month} 2026", out var parsedDate))
                    return BadRequest("Type in a valid month or check the spelling.");
                monthNumber = parsedDate.Month;
            }

            if (monthNumber < 1 || monthNumber > 12)
                return BadRequest("Month number must be between 1 and 12.");

            var currentYear = DateTime.Now.Year;
            if (year < currentYear || year > currentYear + 2)
                return BadRequest($"Year must be between {currentYear} and {currentYear + 2}");

            var bookings = await _bookingService.GetMonthlyBookingsAsync(year, month);
            if (!bookings.Any())
                return NotFound("No booking this month");
            return Ok(bookings);
        }

        [HttpPost("PlaceBooking")]
        public async Task<ActionResult> PlaceBooking(PlaceBookingRequest request)
        {
            var result = await _bookingService.PlaceBookingAsync(request);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.ContactDetailsTaken => BadRequest("This email or phone number already belongs to another guest."),
                    ErrorType.FullyBooked => BadRequest("This time is fully booked, please choose another time."),
                    _ => BadRequest()
                };
            }

            return Ok(new
            {
                Message = "Thank you, your booking has been received!",
                BookingId = result.BookingId
            });
        }

        [HttpGet("{id}/date", Name = "GetBookingDate")]
        public async Task<ActionResult> GetBookingDate(int id)
        {
            var booking = await _bookingService.GetBookingDateAsync(id);
            if (booking == null)
                return NotFound("No booking with this id");
            return Ok(booking);
        }

        [HttpGet("GetBookingsByEmail/{email}", Name = "GetBookingsByEmail")]
        public async Task<ActionResult> GetBookingsByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var bookings = await _bookingService.GetBookingsByEmailAsync(email);
            if (!bookings.Any())
                return NotFound($"No bookings found for the email: {email}");
            return Ok(bookings);
        }

        [HttpGet("ViewBookingsByTime")]
        public async Task<ActionResult> ViewBookingsByTime([FromQuery] DateOnly date, [FromQuery] string time)
        {
            if (!TimeOnly.TryParse(time, out _))
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");

            var tableStatuses = await _bookingService.ViewBookingsByTimeAsync(date, time);
            return Ok(tableStatuses);
        }

        [HttpPut("{id}/editStatus", Name = "EditBookingStatus")]
        public async Task<ActionResult> EditBookingStatus(int id, string request)
        {
            var result = await _bookingService.EditBookingStatusAsync(id, request);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.BookingNotFound => NotFound("This booking does not exist"),
                    ErrorType.InvalidInput => BadRequest($"Invalid booking status. '{request}' is not allowed."),
                    ErrorType.AlreadyCanceled => BadRequest("This booking is already canceled."),
                    ErrorType.AlreadyConfirmed => BadRequest("This booking is already confirmed."),
                    ErrorType.AlreadyComplete => BadRequest("This booking is already complete."),
                    ErrorType.AlreadyPending => BadRequest("This booking is already pending."),
                    var unknownError => throw new InvalidOperationException($"Unhandled error type: {unknownError}")
                };
            }

            return Ok("Booking status updated.");
        }

        [HttpPut("{id}/date", Name = "ChangeBookingDate")]
        public async Task<ActionResult> ChangeBookingDate(int id, BookingDateChangeDto request)
        {
            if (!TimeOnly.TryParse(request.NewStartTime, out _))
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");

            var startDateTime = request.NewBookingDate.ToDateTime(TimeOnly.Parse(request.NewStartTime));
            if (startDateTime <= DateTime.Now)
                return BadRequest("The new booking date and time must be in the future.");

            var result = await _bookingService.ChangeBookingDateAsync(id, request);
            if (!result.Success)
                return result.ErrorType switch
                {
                    ErrorType.BookingNotFound => NotFound("This booking does not exist"),
                    ErrorType.FullyBooked => BadRequest("This requested time is fully booked, please choose another available time"),
                    var unknownError => throw new InvalidOperationException($"Unhandled error type: {unknownError}")
                };
            return Ok("Booking successfully changed.");

        }

        [HttpPut("{id}/details", Name = "UpdateBookingDetails")]
        public async Task<ActionResult> UpdateBookingDetails(int id, UpdateBookingDetailsRequest request)
        {
            var result = await _bookingService.UpdateBookingDetailsAsync(id, request);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    // _ means basically the same as default: 
                    ErrorType.BookingNotFound => NotFound("This booking does not exist"),
                    _ => BadRequest("Could not update booking details.")
                };
            }

            return Ok("Booking details successfully updated.");
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Delete booking by id")]
        public async Task<ActionResult> DeleteGuest(int id)
        {
            var result = await _bookingService.DeleteBookingByIdAsync(id);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.GuestNotFound => NotFound("No booking found with this id."),
                    _ => BadRequest()
                };
            }
            return Ok("Booking deleted.");
        }
    }
}