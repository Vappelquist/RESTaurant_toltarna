using Microsoft.AspNetCore.Mvc;
using Restaurant.Models.Models;
using static Restaurant.API.DTOs.GuestDTOs;

namespace Restaurant.API.Services
{
    public interface IGuestService
    {
        Task<ServiceResult<Guest>> AddGuestAsync([FromBody]CreateAddGuestRequest addGuestRequest);
        Task<List<Guest>> GetAllGuestsAsync();
    }
}
