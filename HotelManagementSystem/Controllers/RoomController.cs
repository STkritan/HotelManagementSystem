using Microsoft.AspNetCore.Mvc;
using HotelManagementSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms.Where(r => r.IsAvailable).ToList();
            return View(rooms);
        }

        public IActionResult Details(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }
    }
}

