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
    }
}
