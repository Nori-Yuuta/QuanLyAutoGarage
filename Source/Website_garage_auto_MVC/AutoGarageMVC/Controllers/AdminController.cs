namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly ImageUploadService _img;
    private readonly ExcelImportService _excel;
    private readonly PasswordHasher _hasher;

    public AdminController(
        AutoGarageDbContext db,
        ImageUploadService img,
        ExcelImportService excel,
        PasswordHasher hasher)
    {
        _db = db;
        _img = img;
        _excel = excel;
        _hasher = hasher;
    }

    // 1. DASHBOARD
    public IActionResult Dashboard()
    {
        ViewBag.Revenue = _db.Orders.Sum(o => (decimal?)o.Total) ?? 0;
        ViewBag.OrderCount = _db.Orders.Count();
        ViewBag.CarCount = _db.Cars.Count();
        ViewBag.AccCount = _db.Accessories.Count();
        ViewBag.RecentOrders = _db.Orders.OrderByDescending(o => o.CreatedAt).Take(8).ToList();
        return View();
    }

    // 2. CARS
    public IActionResult Cars()
    {
        var cars = _db.Cars.OrderByDescending(c => c.Id).ToList();
        return View(cars);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCar(Car model, IFormFile? imageFile, IFormFile? excelFile)
    {
        if (excelFile is { Length: > 0 })
        {
            var rows = _excel.ReadFirstSheet(excelFile.OpenReadStream());
            foreach (var r in rows)
            {
                var car = new Car
                {
                    Name = r.GetValueOrDefault("Tên xe") ?? r.GetValueOrDefault("Name") ?? "Xe Mới",
                    Brand = r.GetValueOrDefault("Hãng") ?? r.GetValueOrDefault("Brand") ?? "Khác",
                    Price = r.GetValueOrDefault("Giá") ?? r.GetValueOrDefault("Price") ?? "0 ₫",
                    ImageUrl = r.GetValueOrDefault("Hình") ?? r.GetValueOrDefault("ImageUrl") ?? "🚗",
                    Year = int.TryParse(r.GetValueOrDefault("Năm"), out var y) ? y : DateTime.UtcNow.Year,
                    Engine = r.GetValueOrDefault("Động cơ"),
                    Fuel = r.GetValueOrDefault("Nhiên liệu"),
                    Seats = int.TryParse(r.GetValueOrDefault("Số chỗ"), out var s) ? s : 5,
                    Transmission = r.GetValueOrDefault("Hộp số"),
                    Origin = r.GetValueOrDefault("Xuất xứ"),
                    Description = r.GetValueOrDefault("Mô tả")
                };
                _db.Cars.Add(car);
            }
            _db.SaveChanges();
            TempData["ToastType"] = "success";
            TempData["ToastTitle"] = "Nhập Excel thành công";
            TempData["ToastMsg"] = $"Đã nhập {rows.Count} xe từ file Excel.";
            return RedirectToAction("Cars");
        }

        var path = await _img.SaveAsync(imageFile);
        if (path != null) model.ImageUrl = path;

        if (model.Id == 0)
        {
            if (string.IsNullOrEmpty(model.ImageUrl)) model.ImageUrl = "🚗";
            _db.Cars.Add(model);
            TempData["ToastMsg"] = "Thêm xe mới thành công!";
        }
        else
        {
            var existing = _db.Cars.Find(model.Id);
            if (existing != null)
            {
                existing.Name = model.Name;
                existing.Brand = model.Brand;
                existing.Price = model.Price;
                if (!string.IsNullOrEmpty(path)) existing.ImageUrl = path;
                existing.Year = model.Year;
                existing.Engine = model.Engine;
                existing.Fuel = model.Fuel;
                existing.Seats = model.Seats;
                existing.Transmission = model.Transmission;
                existing.Origin = model.Origin;
                existing.Description = model.Description;
            }
            TempData["ToastMsg"] = "Cập nhật thông tin xe thành công!";
        }

        _db.SaveChanges();
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        return RedirectToAction("Cars");
    }

    [HttpPost]
    public IActionResult DeleteCar(int id)
    {
        var c = _db.Cars.Find(id);
        if (c != null)
        {
            _db.Cars.Remove(c);
            _db.SaveChanges();
            TempData["ToastType"] = "info";
            TempData["ToastTitle"] = "Đã xóa";
            TempData["ToastMsg"] = $"Đã xóa xe {c.Name}";
        }
        return RedirectToAction("Cars");
    }

    // 3. ACCESSORIES
    public IActionResult Accessories()
    {
        var accs = _db.Accessories.OrderByDescending(a => a.Id).ToList();
        return View(accs);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAccessory(Accessory model, IFormFile? imageFile, IFormFile? excelFile)
    {
        if (excelFile is { Length: > 0 })
        {
            var rows = _excel.ReadFirstSheet(excelFile.OpenReadStream());
            foreach (var r in rows)
            {
                var acc = new Accessory
                {
                    Name = r.GetValueOrDefault("Tên phụ kiện") ?? r.GetValueOrDefault("Name") ?? "Phụ Kiện Mới",
                    Category = r.GetValueOrDefault("Danh mục") ?? r.GetValueOrDefault("Category") ?? "Nội thất",
                    Price = r.GetValueOrDefault("Giá") ?? r.GetValueOrDefault("Price") ?? "0 ₫",
                    ImageUrl = r.GetValueOrDefault("Hình") ?? r.GetValueOrDefault("ImageUrl") ?? "🔧",
                    Description = r.GetValueOrDefault("Mô tả"),
                    Material = r.GetValueOrDefault("Chất liệu"),
                    Warranty = r.GetValueOrDefault("Bảo hành"),
                    Compat = r.GetValueOrDefault("Tương thích")
                };
                _db.Accessories.Add(acc);
            }
            _db.SaveChanges();
            TempData["ToastType"] = "success";
            TempData["ToastTitle"] = "Nhập Excel thành công";
            TempData["ToastMsg"] = $"Đã nhập {rows.Count} phụ kiện từ Excel.";
            return RedirectToAction("Accessories");
        }

        var path = await _img.SaveAsync(imageFile);
        if (path != null) model.ImageUrl = path;

        if (model.Id == 0)
        {
            if (string.IsNullOrEmpty(model.ImageUrl)) model.ImageUrl = "🔧";
            _db.Accessories.Add(model);
            TempData["ToastMsg"] = "Thêm phụ kiện mới thành công!";
        }
        else
        {
            var existing = _db.Accessories.Find(model.Id);
            if (existing != null)
            {
                existing.Name = model.Name;
                existing.Category = model.Category;
                existing.Price = model.Price;
                if (!string.IsNullOrEmpty(path)) existing.ImageUrl = path;
                existing.Description = model.Description;
                existing.Material = model.Material;
                existing.Warranty = model.Warranty;
                existing.Compat = model.Compat;
            }
            TempData["ToastMsg"] = "Cập nhật phụ kiện thành công!";
        }

        _db.SaveChanges();
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        return RedirectToAction("Accessories");
    }

    [HttpPost]
    public IActionResult DeleteAccessory(int id)
    {
        var a = _db.Accessories.Find(id);
        if (a != null)
        {
            _db.Accessories.Remove(a);
            _db.SaveChanges();
            TempData["ToastType"] = "info";
            TempData["ToastTitle"] = "Đã xóa";
            TempData["ToastMsg"] = $"Đã xóa phụ kiện {a.Name}";
        }
        return RedirectToAction("Accessories");
    }

    // 4. NOTIFICATIONS
    public IActionResult Notifications()
    {
        var list = _db.Notifications.OrderByDescending(n => n.Date).ToList();
        return View(list);
    }

    [HttpPost]
    public IActionResult SaveNotification(Notification model)
    {
        if (model.Id == 0)
        {
            model.Date = DateTime.UtcNow;
            _db.Notifications.Add(model);
            TempData["ToastMsg"] = "Thêm thông báo mới thành công!";
        }
        else
        {
            var existing = _db.Notifications.Find(model.Id);
            if (existing != null)
            {
                existing.Title = model.Title;
                existing.Content = model.Content;
            }
            TempData["ToastMsg"] = "Cập nhật thông báo thành công!";
        }

        _db.SaveChanges();
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        return RedirectToAction("Notifications");
    }

    [HttpPost]
    public IActionResult DeleteNotification(int id)
    {
        var n = _db.Notifications.Find(id);
        if (n != null)
        {
            _db.Notifications.Remove(n);
            _db.SaveChanges();
            TempData["ToastType"] = "info";
            TempData["ToastTitle"] = "Đã xóa";
            TempData["ToastMsg"] = "Đã xóa thông báo";
        }
        return RedirectToAction("Notifications");
    }

    // 5. EVENTS
    public IActionResult Events()
    {
        var list = _db.Events.OrderByDescending(e => e.Id).ToList();
        return View(list);
    }

    [HttpPost]
    public IActionResult SaveEvent(Event model)
    {
        if (model.Id == 0)
        {
            _db.Events.Add(model);
            TempData["ToastMsg"] = "Thêm sự kiện mới thành công!";
        }
        else
        {
            var existing = _db.Events.Find(model.Id);
            if (existing != null)
            {
                existing.Name = model.Name;
                existing.Code = model.Code;
                existing.EndDate = model.EndDate;
                existing.Status = model.Status;
            }
            TempData["ToastMsg"] = "Cập nhật sự kiện thành công!";
        }

        _db.SaveChanges();
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        return RedirectToAction("Events");
    }

    [HttpPost]
    public IActionResult DeleteEvent(int id)
    {
        var e = _db.Events.Find(id);
        if (e != null)
        {
            _db.Events.Remove(e);
            _db.SaveChanges();
            TempData["ToastType"] = "info";
            TempData["ToastTitle"] = "Đã xóa";
            TempData["ToastMsg"] = "Đã xóa sự kiện";
        }
        return RedirectToAction("Events");
    }

    // 6. CHAT
    public IActionResult Chat()
    {
        var messages = _db.ChatMessages.OrderBy(m => m.Time).ToList();
        return View(messages);
    }

    [HttpPost]
    public IActionResult SendAdminChat(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            _db.ChatMessages.Add(new ChatMessage
            {
                From = "admin",
                Sender = "AutoGarage Admin",
                SenderEmail = User.Identity?.Name ?? "admin@autogarage.vn",
                Text = text.Trim(),
                Time = DateTime.UtcNow
            });
            _db.SaveChanges();
        }
        return RedirectToAction("Chat");
    }

    // 7. ACCOUNT
    public IActionResult Account()
    {
        var email = User.Identity?.Name;
        var admin = _db.Users.FirstOrDefault(u => u.Email == email && u.Role == "admin")
            ?? _db.Users.FirstOrDefault(u => u.Role == "admin");
        return View(admin);
    }

    [HttpPost]
    public IActionResult UpdateAccount(string name, string phone, string email, string? oldPass, string? newPass)
    {
        var userEmail = User.Identity?.Name;
        var admin = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        if (admin == null) return NotFound();

        admin.Name = name;
        admin.Phone = phone;
        admin.Email = email;

        if (!string.IsNullOrEmpty(newPass))
        {
            if (string.IsNullOrEmpty(oldPass) || !_hasher.Verify(oldPass, admin.PasswordHash))
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Lỗi mật khẩu";
                TempData["ToastMsg"] = "Mật khẩu cũ không chính xác.";
                return RedirectToAction("Account");
            }
            admin.PasswordHash = _hasher.Hash(newPass);
        }

        _db.SaveChanges();
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Thành công!";
        TempData["ToastMsg"] = "Thông tin tài khoản Admin đã được cập nhật.";
        return RedirectToAction("Account");
    }
}
