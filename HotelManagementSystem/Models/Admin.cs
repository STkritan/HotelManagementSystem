using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [StringLength(100)]
        public required string AdminName { get; set; }

        [Required]
        [StringLength(100)]
        public required string Password { get; set; }
    }
}
