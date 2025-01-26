using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HotelManagementSystem.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}

