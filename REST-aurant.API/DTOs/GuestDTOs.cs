namespace REST_aurant.API.DTOs
{
    public class GuestDTOs
    {
        public record CreateAddGuestRequest
        {
            public string? FirstName { get; init; }
            public string? LastName { get; init; }
            public string? PhoneNumber { get; init; }
            public string? Allergies { get; init; }
            public string? Note { get; init; }
        }
    }
}
