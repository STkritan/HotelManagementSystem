﻿using Microsoft.AspNetCore.Mvc;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();
                return View(booking);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

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
            if (room == null)
            {
                ModelState.AddModelError("", "Selected room not found.");
                ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();
                return View(booking);
            }

            booking.TotalPrice = room.PricePerNight * (booking.CheckOutDate - booking.CheckInDate).Days;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(History));
        }

        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == user.Id);

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == user.Id);
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

