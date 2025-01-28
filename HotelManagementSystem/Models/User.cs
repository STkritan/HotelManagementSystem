using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

        public User()
        {
            Bookings = new HashSet<Booking>();
        }
    }
}

