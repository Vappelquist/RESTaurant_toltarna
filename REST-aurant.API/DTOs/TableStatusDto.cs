using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.DTOs
{
    public record TableStatusDto(int TableNumber, int Seats, bool IsAvailable);

    public record AddTableRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "")]
        public int TableNumber { get; set; }
        [Required]
        [Range(1,10, ErrorMessage = "Number of seats must be between 1 and 20")]
        public int Seats { get; set; }
    }
}