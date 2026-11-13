namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.ViewModels;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProfileController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly PasswordHasher _hasher;

    public ProfileController(AutoGarageDbContext db, PasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var email = User.Identity?.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return RedirectToAction("Login", "Auth");

        var orders = _db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserEmail == email)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        var vm = new ProfileVM
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Orders = orders
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult UpdateInfo(ProfileVM vm)
    {
        var email = User.Identity?.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return RedirectToAction("Login", "Auth");

        user.Name = vm.Name;
        user.Phone = vm.Phone;
        _db.SaveChanges();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        TempData["ToastMsg"] = "Thông tin cá nhân đã được cập nhật.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var email = User.Identity?.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return RedirectToAction("Login", "Auth");

        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Lỗi!";
            TempData["ToastMsg"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
            return RedirectToAction("Index");
        }

        if (newPassword != confirmPassword)
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Lỗi!";
            TempData["ToastMsg"] = "Mật khẩu xác nhận không khớp.";
            return RedirectToAction("Index");
        }

        if (!_hasher.Verify(oldPassword, user.PasswordHash))
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Lỗi!";
            TempData["ToastMsg"] = "Mật khẩu hiện tại không chính xác.";
            return RedirectToAction("Index");
        }

        user.PasswordHash = _hasher.Hash(newPassword);
        _db.SaveChanges();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        TempData["ToastMsg"] = "Đổi mật khẩu thành công!";

        return RedirectToAction("Index");
    }
}
