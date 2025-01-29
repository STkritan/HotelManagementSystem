using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly UserManager<User> _userManager;

        public BookingController(HotelDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int? roomId)
        {
            // Fetch available rooms for the dropdown or selection
            ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();

            var booking = new Booking();

            if (roomId.HasValue)
            {
                // Set the RoomId for the booking if it's provided
                booking.RoomId = roomId.Value;
            }
            else
            {
                // Handle case where no roomId is passed
                ModelState.AddModelError("", "No room selected.");
                return View(booking);  // Return the view with the error message
            }

            // If you reach this point, the roomId is valid
            return View(booking);  // Return the booking view (which could contain the roomId)
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                booking.UserId = user.Id;
                booking.BookingDate = DateTime.Now;

                // Check if the room is available for the selected dates
                var isAvailable = !await _context.Bookings
                    .AnyAsync(b => b.RoomId == booking.RoomId && !b.IsCancelled &&
                        ((booking.CheckInDate >= b.CheckInDate && booking.CheckInDate < b.CheckOutDate) ||
                        (booking.CheckOutDate > b.CheckInDate && booking.CheckOutDate <= b.CheckOutDate) ||
                        (booking.CheckInDate <= b.CheckInDate && booking.CheckOutDate >= b.CheckOutDate)));

                if (!isAvailable)
                {
                    ModelState.AddModelError("", "The selected room is not available for the chosen dates.");
                    ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();
                    return View(booking);
                }

                var room = await _context.Rooms.FindAsync(booking.RoomId);
                booking.TotalPrice = room.PricePerNight * (booking.CheckOutDate - booking.CheckInDate).Days;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(History));
            }
            ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();
            return View(booking);
        }

    


        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.IsCancelled = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(History));
        }
    }
}

