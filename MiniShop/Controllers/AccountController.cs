using Application.Common.Interfaces;
using Application.ViewModels;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace MiniShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRedisService _redis;

        // حداکثر ۵ تلاش در ۱۵ دقیقه
        private const int MaxLoginAttempts = 5;
        private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(15);

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRedisService redis)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _redis = redis;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Rate Limit: بررسی تعداد تلاش‌های ناموفق از این IP
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var rateLimitKey = $"login:attempts:{clientIp}";

            var attempts = await _redis.IncrementAsync(rateLimitKey, LockoutWindow);
            if (attempts > MaxLoginAttempts)
            {
                ModelState.AddModelError("", $"تعداد تلاش‌های ناموفق زیاد است. لطفاً {LockoutWindow.Minutes} دقیقه دیگر امتحان کنید.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // ریست کردن counter پس از ورود موفق
                await _redis.RemoveAsync(rateLimitKey);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "ایمیل یا رمز عبور اشتباه است.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
