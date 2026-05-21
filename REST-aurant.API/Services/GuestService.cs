using Restaurant.API.Data;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Services
{
    public class GuestService : IGuestService
    {
        private readonly RestaurantDbContext _context;
        public GuestService(RestaurantDbContext context)
        {
            _context = context;
        }
        public async Task<Guest?> AddGuestAsync(CreateAddGuestRequest addGuestRequest)
        {
            if(addGuestRequest == null)
            {
                return null;
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
            return guestToAdd;
        }
    }
}
