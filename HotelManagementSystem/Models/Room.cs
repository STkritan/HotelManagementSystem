using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    // Room Model
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } // e.g., Single, Double, Suite

        [Required]
        public decimal PricePerNight { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } // Relationship with Booking
    
    }
}
