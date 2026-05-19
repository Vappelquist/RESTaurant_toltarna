using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
