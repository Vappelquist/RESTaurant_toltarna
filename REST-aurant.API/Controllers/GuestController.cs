using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public GuestController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetAllGuests()
        {
            var guests = await _context.Guests.ToListAsync();
            return Ok(guests);
        }

        [HttpPost(Name = "RegisterNewGuest")]
        [EndpointSummary("Register new guest")]
        public async Task<IActionResult> AddGuest(CreateAddGuestRequest addGuestRequest)
        {
            if (addGuestRequest == null)
            {
                return BadRequest("User was not registered.");
            }
            var guestToAdd = new Guest
            {
                FirstName = addGuestRequest.FirstName,
                LastName = addGuestRequest.LastName,
                Email = addGuestRequest.Email,
                Password = addGuestRequest.Password,
                PhoneNumber = addGuestRequest.PhoneNumber,
                Allergies = addGuestRequest.Allergies,
                Note = addGuestRequest.Note
            };
            await _context.Guests.AddAsync(guestToAdd);
            await _context.SaveChangesAsync();
            return Ok(guestToAdd);
        }
    }
}
