using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
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
                    StartTime = b.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    EndTime = b.EndTime.ToString("yyyy-MM-dd HH:mm"),
                    BookingNotes = b.BookingNotes,
                    TableNumbers = b.Tables.Select(t => t.TableNumber).ToList()
                })
                .ToListAsync();
            if(!bookings.Any())
            {
                return NotFound("Det finns ingen bokning");
            }
         
            return Ok(bookings);
        }
    }
}
