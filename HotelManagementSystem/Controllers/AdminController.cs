using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly HotelDbContext _context;

        public AdminController(HotelDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard
        public IActionResult Dashboard()
        {
            var rooms = _context.Rooms.ToList();
            var bookings = _context.Bookings.ToList();
            return View((rooms, bookings));
        }

        // Add or Update Room
        [HttpGet]
        public IActionResult ManageRoom(int? id)
        {
            if (id == null) return View(new Room());
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == id);
            return View(room);
        }

        [HttpPost]
        public IActionResult ManageRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                if (room.RoomId == 0) _context.Rooms.Add(room);
                else _context.Rooms.Update(room);

                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View(room);
        }
    }
}
