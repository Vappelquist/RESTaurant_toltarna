using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestController(RestaurantDbContext context, IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllGuests()
        {
            var guests = await _guestService.GetAllGuestsAsync();
            if (!guests.Any())
            {
                return NotFound("No guests found.");
            }
            return Ok(guests);
        }

        [HttpPost(Name = "RegisterNewGuest")]
        [EndpointSummary("Register new guest")]
        public async Task<IActionResult> AddGuest(CreateAddGuestRequest addGuestRequest)
        {
            var result = await _guestService.AddGuestAsync(addGuestRequest);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.RequestMissing => BadRequest("Request is missing."),
                    ErrorType.InvalidInput => BadRequest("FirstName, LastName and Email has to be filled."),
                    _ => BadRequest()
                };
            }
            
            return Ok(result.Data);
        }
    }
}
