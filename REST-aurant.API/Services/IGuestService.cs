using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Services
{
    public interface IGuestService
    {
        Task<Guest?> AddGuestAsync(CreateAddGuestRequest addGuestRequest);
    }
}
