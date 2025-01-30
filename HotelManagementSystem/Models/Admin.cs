namespace HotelManagementSystem.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public required string UserId { get; set; }
        public required User User { get; set; }
    }
}

