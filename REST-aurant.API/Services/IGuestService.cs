using Microsoft.AspNetCore.Mvc;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Services
{
    public interface IGuestService
    {
        Task<ServiceResult<Guest>> AddGuestAsync([FromBody]CreateAddGuestRequest addGuestRequest);
        Task<List<GetGuestResponse>> GetAllGuestsAsync();
        Task<ServiceResult<Guest>> UpdateGuestAsync(int id, UpdateGuestRequest request);

        Task<Guest?> GetGuestByEmailAsync(string email);
        Task<ServiceResult> DeleteGuestByIdAsync(int id);
    }
}
