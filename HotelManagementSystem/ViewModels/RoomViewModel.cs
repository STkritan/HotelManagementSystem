using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HotelManagementSystem.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        [Required]
        [Display(Name = "Price per Night")]
        [DataType(DataType.Currency)]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Room Image")]
        public IFormFile ImageFile { get; set; }

        public string ExistingImageUrl { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }
    }
}

