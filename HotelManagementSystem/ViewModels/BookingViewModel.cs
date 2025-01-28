using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.ViewModels
{
    public class BookingViewModel
    {
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        [Display(Name = "Price per Night")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
    }
}

