using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HotelManagementSystem.Models; // Replace YourAppName with your actual application name
using HotelManagementSystem.ViewModels; // Replace YourAppName with your actual application name
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data; // Add this using statement

namespace HotelManagementSystem.Controllers // Replace YourAppName with your actual application name
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly HotelDbContext _context; // Add DbContext

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, HotelDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize] // This ensures only logged-in users can access the dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new DashboardViewModel
            {
                TotalBookings = await _context.Bookings
                    .CountAsync(b => b.UserId == user.Id),
                RecentBookings = await _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(3)
                    .ToListAsync(),
                AvailableRooms = await _context.Rooms
                    .Where(r => r.IsAvailable)
                    .Take(6)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}

