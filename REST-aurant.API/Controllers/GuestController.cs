using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Data;
using Restaurant.API.Services;
using Restaurant.API.Services.Enums;
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _guestService.AddGuestAsync(addGuestRequest);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.ContactDetailsTaken => BadRequest("Account is already registed with the E-mail."),
                    _ => BadRequest()
                };
            }

            return Ok(result.Data);
        }

        [HttpPut("{id}", Name = "UpdateGuest")]
        [EndpointSummary("Update existing guest information")]
        public async Task<IActionResult> UpdateGuest(int id, UpdateGuestRequest request)
        {
            var result = await _guestService.UpdateGuestAsync(id, request);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.GuestNotFound => NotFound($"Could not find a guest with ID {id}."),
                    _ => BadRequest("Something went wrong while updating the guest.")
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("{email}")]
        [EndpointSummary("Get guest by email")]
        public async Task<ActionResult> GetGuestByEmail(string email)
        {
            var guest = await _guestService.GetGuestByEmailAsync(email);

            if(guest == null)
            {
                return NotFound("No guest found with this email.");
            }
            return Ok(guest);
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Delete guest by id")]
        public async Task<ActionResult> DeleteGuest(int id)
        {
            var result = await _guestService.DeleteGuestByIdAsync(id);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.GuestNotFound => NotFound("No guest found with this id."),
                    _ => BadRequest()
                };
            }
            return Ok("Guest deleted.");
        }
    }
}