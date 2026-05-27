using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$")]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
