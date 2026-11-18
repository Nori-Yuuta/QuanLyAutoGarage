namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;

public class DepositController : Controller
{
    private readonly AutoGarageDbContext _db;

    public DepositController(AutoGarageDbContext db)
    {
        _db = db;
    }

    // 10. ĐẶT CỌC GIỮ XE TRỰC TUYẾN (ONLINE CAR DEPOSIT)
    [HttpGet]
    public IActionResult Index(int? carId)
    {
        var allCars = _db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Name).ToList();
        ViewBag.AllCars = allCars;
        ViewBag.SelectedCar = carId.HasValue ? allCars.FirstOrDefault(c => c.Id == carId.Value) : allCars.FirstOrDefault();

        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        ViewBag.CustomerUser = user;

        return View();
    }

    [HttpPost]
    public IActionResult CreateDeposit(CarDepositBooking model)
    {
        if (string.IsNullOrWhiteSpace(model.CustomerName) || string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Thiếu thông tin";
            TempData["ToastMsg"] = "Vui lòng nhập Họ tên và Số điện thoại người đặt cọc!";
            return RedirectToAction("Index", new { carId = model.CarId });
        }

        var car = model.CarId.HasValue ? _db.Cars.Find(model.CarId.Value) : null;
        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;

        var depositCode = "DC" + DateTime.Now.ToString("yyMMdd") + new Random().Next(1000, 9999);

        model.DepositCode = depositCode;
        model.UserId = user?.Id;
        model.CarName = car != null ? $"{car.Name} ({car.Brand})" : "Mẫu xe đặt cọc";
        if (model.DepositAmount <= 0) model.DepositAmount = 20_000_000;
        model.PaymentStatus = "Chờ thanh toán";
        model.CreatedAt = DateTime.Now;

        _db.CarDepositBookings.Add(model);

        // Tự động đẩy vào Lead tiềm năng phân hệ Quản trị CRM
        _db.CustomerLeads.Add(new CustomerLead
        {
            FullName = model.CustomerName,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            LeadSource = "Đặt cọc giữ xe trực tuyến",
            InterestedCarOrService = $"{model.CarName} (Cọc: {model.DepositAmount:N0} ₫)",
            Stage = "Đàm phán",
            Notes = $"Mã cọc: {depositCode}. Phương thức: {model.PaymentMethod}. Yêu cầu: {model.Note}",
            ExpectedValue = model.DepositAmount,
            CreatedAt = DateTime.Now
        });

        _db.SaveChanges();

        return RedirectToAction("Payment", new { depositCode = depositCode });
    }

    [HttpGet]
    public IActionResult Payment(string depositCode)
    {
        var deposit = _db.CarDepositBookings.FirstOrDefault(d => d.DepositCode == depositCode);
        if (deposit == null) return NotFound();

        return View(deposit);
    }

    [HttpPost]
    public IActionResult ConfirmPaymentSuccess(string depositCode, string? transactionRef)
    {
        var deposit = _db.CarDepositBookings.FirstOrDefault(d => d.DepositCode == depositCode);
        if (deposit != null)
        {
            deposit.PaymentStatus = "Đã thanh toán";
            deposit.TransactionRef = !string.IsNullOrWhiteSpace(transactionRef) ? transactionRef : "BANK-" + DateTime.Now.Ticks.ToString().Substring(10);
            _db.SaveChanges();

            TempData["ToastType"] = "success";
            TempData["ToastTitle"] = "Đặt cọc thành công!";
            TempData["ToastMsg"] = $"Đã nhận đặt cọc mã {deposit.DepositCode} cho mẫu xe {deposit.CarName}. AutoGarage đã khóa giữ xe cho quý khách!";
        }

        return RedirectToAction("Payment", new { depositCode = depositCode });
    }
}
