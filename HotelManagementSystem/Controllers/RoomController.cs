using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomController(HotelDbContext context)
        {
            _context = context;
        }

        // List all rooms
        public IActionResult Index()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }

        // Details of a specific room
        public IActionResult Details(int id)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == id);
            if (room == null) return NotFound();
            return View(room);
        }

        // Admin: Manage Rooms
        public IActionResult Manage()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }
    }
}
