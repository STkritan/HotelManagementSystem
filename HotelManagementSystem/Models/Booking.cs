using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public bool IsCancelled { get; set; } = false;

        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}

