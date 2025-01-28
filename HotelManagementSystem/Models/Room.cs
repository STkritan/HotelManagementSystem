using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Required]
        public string RoomType { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string Description { get; set; }

        [Display(Name = "Room Image")]
        public string ImageUrl { get; set; }
    }
}

