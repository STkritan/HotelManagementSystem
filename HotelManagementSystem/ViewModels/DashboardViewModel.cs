using HotelManagementSystem.Models;
using System.Collections.Generic;

namespace HotelManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<Booking> RecentBookings { get; set; } = new List<Booking>();
        public IEnumerable<Room> AvailableRooms { get; set; } = new List<Room>();
    }
}

