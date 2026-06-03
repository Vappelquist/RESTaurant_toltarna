using Restaurant.Models.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.DTOs
{
    public class GuestDTOs
    {
        public record GuestBaseDTO
        {

            [Required]
            [MinLength(2)]
            public string? FirstName { get; init; }

            [Required]
            [MinLength(2)]
            public string? LastName { get; init; }

            [Required]
            [EmailAddress]
            public string? Email { get; init; }

            [Phone]
            public string? PhoneNumber { get; init; }

            public string? Allergies { get; init; }

            public string? Note { get; init; }


        }
        public record CreateAddGuestRequest : GuestBaseDTO
        {

            [Required]
            [MinLength(8)]
            public string? Password { get; init; }

        }

        public record UpdateGuestRequest : GuestBaseDTO
        {

        }

        public record GetGuestResponse : GuestBaseDTO
        {
            public int Id { get; init; }
            public List<BookingStatus> BookingStatuses { get; init; } = new();
        }
    }
}
