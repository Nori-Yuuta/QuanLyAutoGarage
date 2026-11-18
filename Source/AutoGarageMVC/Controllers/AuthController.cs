namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Models.ViewModels;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly AuthService _auth;

    public AuthController(AutoGarageDbContext db, PasswordHasher hasher, AuthService auth)
    {
        _db = db;
        _hasher = hasher;
        _auth = auth;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == vm.Email.ToLower().Trim());
        if (user == null || !_hasher.Verify(vm.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View(vm);
        }

        await _auth.SignInAsync(user, vm.RememberMe);

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Đăng nhập thành công!";
        TempData["ToastMsg"] = $"Xin chào {user.Name}";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return user.Role == "admin"
            ? RedirectToAction("Dashboard", "Admin")
            : RedirectToAction("Index", "Profile");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var normalizedEmail = vm.Email.ToLower().Trim();
        if (_db.Users.Any(u => u.Email.ToLower() == normalizedEmail))
        {
            ModelState.AddModelError("Email", "Email đã được đăng ký trong hệ thống.");
            return View(vm);
        }

        var user = new User
        {
            Name = vm.Name.Trim(),
            Email = normalizedEmail,
            Phone = vm.Phone.Trim(),
            PasswordHash = _hasher.Hash(vm.Password),
            Role = "customer"
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        await _auth.SignInAsync(user);

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Đăng ký thành công!";
        TempData["ToastMsg"] = $"Chào mừng {user.Name} đến với AutoGarage";

        return RedirectToAction("Index", "Profile");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _auth.SignOutAsync();
        TempData["ToastType"] = "info";
        TempData["ToastTitle"] = "Đã đăng xuất";
        TempData["ToastMsg"] = "Hẹn gặp lại bạn!";
        return RedirectToAction("Index", "Home");
    }
}
