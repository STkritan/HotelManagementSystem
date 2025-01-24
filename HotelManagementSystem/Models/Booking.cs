using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }
        public required virtual Room Room { get; set; } // Navigation property

        [Required]
        public int UserId { get; set; }
        public required User User { get; set; } // Navigation property

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public bool IsCancelled { get; set; } = false;

        [Required]
        public DateTime BookingDate { get; set; }
    }
}
