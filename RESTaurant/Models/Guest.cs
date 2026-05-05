using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant.Models
{
    internal class Guest : User
    {
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
        public bool IsEighteen { get; set; }
        public string? Allergies { get; set; }
        public string? Note { get; set; }

    }
}
