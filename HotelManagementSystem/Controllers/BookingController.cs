using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Collections;

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Booking booking)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.GetUserAsync(User);
        booking.UserId = user.Id;  // Now this will work as both are strings
        booking.BookingDate = DateTime.Now;
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var room = await _context.Rooms.FindAsync(booking.RoomId);
        if (room != null)
        {
            room.IsAvailable = false;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(History));
    }
    ViewBag.Rooms = await _context.Rooms.Where(r => r.IsAvailable).ToListAsync();
    return View(booking);
}

