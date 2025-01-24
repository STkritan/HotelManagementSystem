using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelDbContext _context;

        public BookingController(HotelDbContext context)
        {
            _context = context;
        }

        // Book a Room
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Rooms = _context.Rooms.Where(r => r.IsAvailable).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                _context.Bookings.Add(booking);

                // Update room availability
                var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
                if (room != null) room.IsAvailable = false;

                _context.SaveChanges();
                return RedirectToAction("History", "Booking");
            }
            return View(booking);
        }

        // View Booking History
        public IActionResult History(int userId)
        {
            var bookings = _context.Bookings.Where(b => b.UserId == userId).ToList();
            return View(bookings);
        }

        // Cancel Booking
        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost]
        public IActionResult CancelConfirmed(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                booking.IsCancelled = true;

                // Mark room as available
                var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
                if (room != null) room.IsAvailable = true;

                _context.SaveChanges();
            }
            return RedirectToAction("History");
        }
    }
}
