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
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
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

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

