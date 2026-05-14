using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
using Restaurant.Models.Models.Enums;
using static REST_aurant.API.DTOs.Booking;

namespace REST_aurant.API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelBookingController : ControllerBase
    {
        private readonly RestaurantDbContext _ctx;
        public CancelBookingController(RestaurantDbContext ctx)
        {
            _ctx = ctx;
        }
        [HttpPut("{id}", Name = "CancelBooking")]
        public async Task<ActionResult> CancelBooking(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Bokningen finns inte");
            }
            if (booking.Status == BookingStatus.Canceled)
            {
                return BadRequest("Bokningen är redan avbokad");
            }
            booking.Status = BookingStatus.Canceled;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}