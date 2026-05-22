using Restaurant.API.Data;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Restaurant.API.Services
{
    public class GuestService : IGuestService
    {
        private readonly RestaurantDbContext _context;
        public GuestService(RestaurantDbContext context)
        {
            _context = context;
        }
        public async Task<(Guest?guest, string? error)> AddGuestAsync(CreateAddGuestRequest addGuestRequest)
        {
            if (addGuestRequest == null)
            {
                return (null, "Request is missing");
            }

            if (string.IsNullOrWhiteSpace(addGuestRequest.FirstName) || string.IsNullOrWhiteSpace(addGuestRequest.LastName))
            {
                return (null, "FirstName and LastName has to be filled");
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
            return (guestToAdd, null);
        }
    }
}
