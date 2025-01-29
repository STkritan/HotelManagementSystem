using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModels;
using HotelManagementSystem.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HotelDbContext _context;

        public AdminController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            HotelDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction(nameof(Dashboard));
                    }

                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "You are not authorized as an admin.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Dashboard()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .OrderByDescending(b => b.BookingDate)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalRooms = await _context.Rooms.CountAsync();
            ViewBag.AvailableRooms = await _context.Rooms.CountAsync(r => r.IsAvailable);
            ViewBag.TotalRevenue = await _context.Bookings
                .Where(b => !b.IsCancelled)
                .SumAsync(b => b.TotalPrice);

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ManageRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        public IActionResult CreateRoom()
        {
            return View(new Room());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageRooms));
            }
            return View(room);
        }

        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(int id, Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageRooms));
            }
            return View(room);
        }

        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost, ActionName("DeleteRoom")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoomConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageRooms));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}

