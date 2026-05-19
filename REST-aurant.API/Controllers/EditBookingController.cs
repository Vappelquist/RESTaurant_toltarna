using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.DTOs;
using Restaurant.API.Services;
using Restaurant.API.Data;
using Restaurant.Models.Models.Enums;

namespace Restaurant.API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class EditBookingController : ControllerBase
    {
        private readonly RestaurantDbContext _ctx;
        private readonly ITableService _tableService;
        public EditBookingController(RestaurantDbContext ctx, ITableService tableService)
        {   
            _ctx = ctx;
            _tableService = tableService;
        }

        //Use this endpoint to cancel a booking if the guest calls to cancel.
        [HttpPut("{id}/cancel", Name = "CancelBooking")]
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

        //Use this endpoint to revert accidentally canceled bookings back to confirmed status.
        [HttpPut("{id}/confirm", Name = "ConfirmBooking")]
        public async Task<ActionResult> ConfirmBooking(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Bokningen finns inte");
            }
            if (booking.Status == BookingStatus.Confirmed)
            {
                return BadRequest("Bokningen är redan bekräftad");
            }
            booking.Status = BookingStatus.Confirmed;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        //Use this endpoint to mark a booking as complete after the guests have finished dining.
        [HttpPut("{id}/complete", Name = "CompleteBooking")]
        public async Task<ActionResult> CompleteBooking(int id)
        {
            var booking = await _ctx.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Bokningen finns inte");
            }
            if (booking.Status == BookingStatus.Complete)
            {
                return BadRequest("Bokningen är redan slutförd");
            }
            booking.Status = BookingStatus.Complete;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        // Use this endpoint to try and change the date of the booking
        [HttpPut("{id}/date", Name = "ChangeBookingDate")]
        public async Task<ActionResult> ChangeBookingDate(int id, BookingDateChangeDto request)
        {
            // 1. FIX: Lägg till .Include(b => b.Tables) så EF kan hantera relationsändringen
            var booking = await _ctx.Bookings
                .Include(b => b.Tables)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound("This booking does not exist");
            }

            // Validate the new start time format
            if (!TimeOnly.TryParse(request.NewStartTime, out var startTime))
            {
                return BadRequest("Time must be entered in format HH:mm. For example 18:30");
            }

            // Check if the new booking date and time is in the future
            var startDateTime = request.NewBookingDate.ToDateTime(startTime);
            if (startDateTime <= DateTime.Now)
            {
                return BadRequest("The new booking date and time must be in the future.");
            }

            var endTime = startDateTime.AddHours(2);
            var allocatedTables = await _tableService.AllocateTablesAsync(startDateTime, endTime, booking.AmountOfGuests, id);

            // If the list is empty, it means there are no or not enough available tables for the requested time slot
            if (!allocatedTables.Any())
            {
                return BadRequest("This requested time is fully booked, please choose another available time.");
            }

            // Uppdatera bokningen
            booking.StartTime = startDateTime;
            booking.EndTime = endTime;
            booking.Tables = allocatedTables; 

            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}