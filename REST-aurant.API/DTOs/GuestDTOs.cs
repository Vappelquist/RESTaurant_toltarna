using Restaurant.Models.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.DTOs
{
    public class GuestDTOs
    {
        public record CreateAddGuestRequest
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
            [Required]
            [MinLength(8)]

            public string? Password { get; init; }
            [Phone]
            public string? PhoneNumber { get; init; }
            public string? Allergies { get; init; }
            public string? Note { get; init; }
        }

        public record UpdateGuestRequest
        {
            [MinLength(2)]
            public string? FirstName { get; init; }

            [MinLength(2)]
            public string? LastName { get; init; }

            [EmailAddress]
            public string? Email { get; init; }

            [Phone]
            public string? PhoneNumber { get; init; }

            public string? Allergies { get; init; }
            public string? Note { get; init; }
        }

        public record GetGuestResponse
        {
            public int Id { get; init; }
            public string? FirstName { get; init; }
            public string? LastName { get; init; }
            public string? Email { get; init; }
            public string? PhoneNumber { get; init; }
            public string? Allergies { get; init; }
            public string? Note { get; init; }
            public List<BookingStatus> BookingStatuses { get; init; } = new();
        }
    }
}
