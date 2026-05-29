using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Guest>> GetAllGuestsAsync()
        {
            return await _context.Guests.ToListAsync();
        }
        public async Task<ServiceResult<Guest>> AddGuestAsync(CreateAddGuestRequest addGuestRequest)
        {
            var exists = await _context.Guests.AnyAsync(g => g.Email == addGuestRequest.Email);
            if (exists)
            {
                return new ServiceResult<Guest> { Success = false, ErrorType = Enums.ErrorType.ContactDetailsTaken };
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
            return new ServiceResult<Guest>
            {
                Success = true,
                Data = guestToAdd
            };
        }
    }
}
