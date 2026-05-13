using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
using Restaurant.Models.Models;

namespace REST_aurant.API.Controllers
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

        [HttpPost]
        public async Task<IActionResult> AddGuest(Guest guest)
        {
            if (guest == null)
            {
                return BadRequest("User was not registered.");
            }
            var guestToAdd = new Guest
            {
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PhoneNumber = guest.PhoneNumber,
                Allergies = guest.Allergies,
                Note = guest.Note
            };
            await _context.Guests.AddAsync(guestToAdd);
            await _context.SaveChangesAsync();
            return Ok(guestToAdd);
        }
    }
}
