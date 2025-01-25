using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelManagementSystem.Models;
using System.Linq;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalBookings = _context.Bookings.Count();
            ViewBag.AvailableRooms = _context.Rooms.Count(r => r.IsAvailable);
            ViewBag.TotalRevenue = _context.Bookings.Sum(b => b.Room.PricePerNight * (b.CheckOutDate - b.CheckInDate).Days);

            var recentBookings = _context.Bookings
                .OrderByDescending(b => b.BookingDate)
                .Take(10)
                .ToList();

            return View(recentBookings);
        }

        public IActionResult ManageRoom(int? id)
        {
            if (id == null)
            {
                return View(new Room());
            }

            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                if (room.RoomId == 0)
                {
                    _context.Rooms.Add(room);
                }
                else
                {
                    _context.Rooms.Update(room);
                }
                _context.SaveChanges();
                return RedirectToAction("Manage", "Room");
            }
            return View(room);
        }

        public IActionResult DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction("Manage", "Room");
        }
    }
}

