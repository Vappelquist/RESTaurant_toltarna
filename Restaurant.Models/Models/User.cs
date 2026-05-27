using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
