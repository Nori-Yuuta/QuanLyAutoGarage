namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class ServicesController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly ImageUploadService _img;

    public ServicesController(AutoGarageDbContext db, ImageUploadService img)
    {
        _db = db;
        _img = img;
    }

    // ==========================================
    // 6. ĐẶT LỊCH BẢO DƯỠNG / SỬA CHỮA
    // ==========================================
    [HttpGet]
    public IActionResult Appointment()
    {
        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        ViewBag.CustomerUser = user;
        return View();
    }

    [HttpPost]
    public IActionResult BookAppointment(ServiceAppointment appointment)
    {
        if (string.IsNullOrWhiteSpace(appointment.CustomerName) || 
            string.IsNullOrWhiteSpace(appointment.PhoneNumber) || 
            string.IsNullOrWhiteSpace(appointment.LicensePlate))
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Thiếu thông tin";
            TempData["ToastMsg"] = "Vui lòng nhập Họ tên, Số điện thoại và Biển số xe!";
            return RedirectToAction("Appointment");
        }

        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        appointment.UserId = user?.Id;
        appointment.CreatedAt = DateTime.Now;
        appointment.Status = "Chờ tiếp nhận";

        _db.ServiceAppointments.Add(appointment);

        // Tự động thêm vào luồng Khách hàng tiềm năng (Leads) của Garage
        _db.CustomerLeads.Add(new CustomerLead
        {
            FullName = appointment.CustomerName,
            PhoneNumber = appointment.PhoneNumber,
            LeadSource = "Đặt lịch bảo dưỡng",
            InterestedCarOrService = $"{appointment.ServicePackage} ({appointment.LicensePlate} - {appointment.CarBrandAndModel})",
            Stage = "Mới tiếp nhận",
            Notes = $"Khung giờ: {appointment.AppointmentDate:dd/MM/yyyy} ({appointment.TimeSlot}). Yêu cầu: {appointment.CustomerNotes}",
            CreatedAt = DateTime.Now
        });

        _db.SaveChanges();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Đặt lịch thành công!";
        TempData["ToastMsg"] = $"AutoGarage đã ghi nhận lịch bảo dưỡng xe biển số {appointment.LicensePlate} vào ngày {appointment.AppointmentDate:dd/MM/yyyy}. Cố vấn dịch vụ sẽ gọi xác nhận ngay!";

        return RedirectToAction("Appointment");
    }

    // ==========================================
    // 7. ĐỊNH GIÁ XE CŨ & THU CŨ ĐỔI MỚI (TRADE-IN)
    // ==========================================
    [HttpGet]
    public IActionResult Valuation()
    {
        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        ViewBag.CustomerUser = user;
        ViewBag.NewCars = _db.Cars.OrderBy(c => c.Name).ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitValuation(TradeInValuation model, IFormFile? carPhoto)
    {
        if (string.IsNullOrWhiteSpace(model.CustomerName) || 
            string.IsNullOrWhiteSpace(model.PhoneNumber) || 
            string.IsNullOrWhiteSpace(model.CurrentBrand) || 
            string.IsNullOrWhiteSpace(model.CurrentModel))
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Thiếu thông tin";
            TempData["ToastMsg"] = "Vui lòng nhập đầy đủ thông tin liên hệ và mẫu xe hiện tại!";
            return RedirectToAction("Valuation");
        }

        if (carPhoto != null)
        {
            var uploadedPath = await _img.SaveAsync(carPhoto);
            if (uploadedPath != null) model.ImageUrl = uploadedPath;
        }

        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        model.UserId = user?.Id;
        model.CreatedAt = DateTime.Now;
        model.Status = "Chờ thẩm định";

        _db.TradeInValuations.Add(model);

        // Tự động ghi nhận Lead tiềm năng (Thu cũ đổi mới)
        _db.CustomerLeads.Add(new CustomerLead
        {
            FullName = model.CustomerName,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            LeadSource = "Thu cũ đổi mới",
            InterestedCarOrService = $"Bán: {model.CurrentBrand} {model.CurrentModel} ({model.ManufactureYear}) -> Đổi: {model.TargetCarModel ?? "Chưa chọn xe mới"}",
            Stage = "Mới tiếp nhận",
            Notes = $"ODO: {model.MileageKm:N0} Km. Tình trạng: {model.ConditionDescription}",
            CreatedAt = DateTime.Now
        });

        _db.SaveChanges();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Gửi thông tin thành công!";
        TempData["ToastMsg"] = $"Hồ sơ định giá xe {model.CurrentBrand} {model.CurrentModel} đã được gửi tới chuyên viên thẩm định. Chúng tôi sẽ gửi kết quả định giá sớm nhất!";

        return RedirectToAction("Valuation");
    }

    // ==========================================
    // 8. TRA CỨU LỊCH SỬ BẢO HÀNH / BẢO DƯỠNG
    // ==========================================
    [HttpGet]
    public IActionResult History(string? query)
    {
        List<ServiceHistoryRecord> results = new();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim().Replace("-", "").Replace(".", "").Replace(" ", "").ToLower();
            results = _db.ServiceHistoryRecords
                .AsEnumerable()
                .Where(r => 
                    (r.LicensePlate != null && r.LicensePlate.Replace("-", "").Replace(".", "").Replace(" ", "").ToLower().Contains(q)) ||
                    (r.VinNumber != null && r.VinNumber.Trim().ToLower().Contains(q.ToLower())) ||
                    (r.PhoneNumber != null && r.PhoneNumber.Contains(q))
                )
                .OrderByDescending(r => r.ServiceDate)
                .ToList();
        }

        ViewBag.Query = query;
        return View(results);
    }
}
