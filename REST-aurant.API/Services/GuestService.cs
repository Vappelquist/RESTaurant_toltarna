using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Services.Enums;
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
        public async Task<List<GetGuestResponse>> GetAllGuestsAsync()
        {
            return await _context.Guests
                .AsNoTracking()
                .Select(g => new GetGuestResponse
                {
                    Id = g.Id,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    Email = g.Email,
                    PhoneNumber = g.PhoneNumber,
                    BookingStatuses = g.Bookings!.Select(b => b.Status).ToList()
                })
                .ToListAsync();
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

        public async Task<ServiceResult<Guest>> UpdateGuestAsync(int id, UpdateGuestRequest request)
        {
            // finds the guest in the database
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return new ServiceResult<Guest>
                {
                    Success = false,
                    ErrorType = ErrorType.GuestNotFound
                };
            }

            // Only update the fileds that are not empty.
            if (!string.IsNullOrWhiteSpace(request.FirstName)) guest.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) guest.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.Email)) guest.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) guest.PhoneNumber = request.PhoneNumber;

            // Let them remove their alergies if they wanna. 
            if (request.Allergies != null) guest.Allergies = request.Allergies;
            if (request.Note != null) guest.Note = request.Note;

            // Save!
            await _context.SaveChangesAsync();

            return new ServiceResult<Guest>
            {
                Success = true,
                Data = guest
            };
        }
        public async Task<Guest?> GetGuestByEmailAsync(string email)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Email == email);
        }

        public async Task<ServiceResult> DeleteGuestByIdAsync(int id)
        {
            //Find guest by email
            var guestToDelete = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);

            //Return error if not found
            if (guestToDelete == null)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorType = ErrorType.GuestNotFound
                };
            }
            _context.Guests.Remove(guestToDelete);
            await _context.SaveChangesAsync();
            return new ServiceResult
            {
                Success = true
            };
        }
    }
}
