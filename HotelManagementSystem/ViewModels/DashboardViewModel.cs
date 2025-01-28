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
        public IEnumerable<Booking> RecentBookings { get; set; }
        public IEnumerable<Room> AvailableRooms { get; set; }
    }
}

