namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;

public class CarsController : Controller
{
    private readonly AutoGarageDbContext _db;

    public CarsController(AutoGarageDbContext db)
    {
        _db = db;
    }

    // 1. DANH SÁCH XE & BỘ LỌC TÌM KIẾM CHUYÊN SÂU
    public IActionResult Index(
        string? brand,
        string? search,
        string? priceRange,
        string? fuel,
        string? transmission,
        int? seats,
        string? sortBy)
    {
        var query = _db.Cars.AsQueryable();

        // Lọc Hãng xe
        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(c => c.Brand.ToLower() == brand.ToLower() || c.Brand.ToLower().Contains(brand.ToLower()));
        }

        // Tìm kiếm theo từ khóa (Tên xe, Hãng)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(s) || c.Brand.ToLower().Contains(s));
        }

        // Lọc Nhiên liệu (Xăng, Dầu, Hybrid, Điện)
        if (!string.IsNullOrWhiteSpace(fuel))
        {
            query = query.Where(c => c.Fuel != null && c.Fuel.ToLower().Contains(fuel.ToLower()));
        }

        // Lọc Hộp số (Tự động, CVT, Số sàn)
        if (!string.IsNullOrWhiteSpace(transmission))
        {
            query = query.Where(c => c.Transmission != null && c.Transmission.ToLower().Contains(transmission.ToLower()));
        }

        // Lọc Số chỗ ngồi
        if (seats.HasValue && seats.Value > 0)
        {
            query = query.Where(c => c.Seats == seats.Value);
        }

        var cars = query.ToList();

        // Lọc Khoảng giá (Parse chuỗi Price thành số)
        if (!string.IsNullOrWhiteSpace(priceRange))
        {
            cars = cars.Where(c =>
            {
                var priceVal = ParsePriceToNumber(c.Price);
                return priceRange switch
                {
                    "under-600" => priceVal < 600_000_000,
                    "600-1000" => priceVal >= 600_000_000 && priceVal <= 1_000_000_000,
                    "1000-1500" => priceVal > 1_000_000_000 && priceVal <= 1_500_000_000,
                    "above-1500" => priceVal > 1_500_000_000,
                    _ => true
                };
            }).ToList();
        }

        // Sắp xếp
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            cars = sortBy switch
            {
                "price-asc" => cars.OrderBy(c => ParsePriceToNumber(c.Price)).ToList(),
                "price-desc" => cars.OrderByDescending(c => ParsePriceToNumber(c.Price)).ToList(),
                "year-desc" => cars.OrderByDescending(c => c.Year).ToList(),
                "name-asc" => cars.OrderBy(c => c.Name).ToList(),
                _ => cars
            };
        }

        ViewBag.Brand = brand;
        ViewBag.Search = search;
        ViewBag.PriceRange = priceRange;
        ViewBag.Fuel = fuel;
        ViewBag.Transmission = transmission;
        ViewBag.Seats = seats;
        ViewBag.SortBy = sortBy;
        ViewBag.AllBrands = new[] { "toyota", "honda", "ford", "mazda", "bmw", "mercedes" };

        return View(cars);
    }

    // 2. SO SÁNH XE (CAR COMPARISON TOOL)
    public IActionResult Compare(int? car1, int? car2, int? car3)
    {
        var allCars = _db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Name).ToList();
        ViewBag.AllCars = allCars;

        // Lấy danh sách các xe được chọn để so sánh
        var selectedIds = new List<int>();
        if (car1.HasValue && car1.Value > 0) selectedIds.Add(car1.Value);
        if (car2.HasValue && car2.Value > 0) selectedIds.Add(car2.Value);
        if (car3.HasValue && car3.Value > 0) selectedIds.Add(car3.Value);

        // Mặc định nếu chưa chọn xe nào, lấy 2 xe đầu tiên để người dùng thấy bảng mẫu
        if (!selectedIds.Any() && allCars.Count >= 2)
        {
            selectedIds.Add(allCars[0].Id);
            selectedIds.Add(allCars[1].Id);
        }

        var comparisonCars = allCars.Where(c => selectedIds.Contains(c.Id)).ToList();
        ViewBag.Car1 = car1 ?? (comparisonCars.Count > 0 ? comparisonCars[0].Id : 0);
        ViewBag.Car2 = car2 ?? (comparisonCars.Count > 1 ? comparisonCars[1].Id : 0);
        ViewBag.Car3 = car3 ?? (comparisonCars.Count > 2 ? comparisonCars[2].Id : 0);

        return View(comparisonCars);
    }

    // 3. DỰ TOÁN TRẢ GÓP & CHI PHÍ LĂN BÁNH (CALCULATOR)
    public IActionResult Calculator(int? carId)
    {
        var allCars = _db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Name).ToList();
        ViewBag.AllCars = allCars;
        ViewBag.SelectedCarId = carId ?? (allCars.FirstOrDefault()?.Id ?? 0);
        return View();
    }

    // 4. ĐĂNG KÝ LÁI THỬ TRỰC TUYẾN (BOOK A TEST DRIVE)
    public IActionResult TestDrive(int? carId)
    {
        var allCars = _db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Name).ToList();
        ViewBag.AllCars = allCars;
        ViewBag.SelectedCarId = carId;

        // Tự động điền thông tin nếu khách hàng đã đăng nhập
        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;
        ViewBag.CustomerUser = user;

        return View();
    }

    [HttpPost]
    public IActionResult BookTestDrive(TestDriveBooking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.CustomerName) || string.IsNullOrWhiteSpace(booking.PhoneNumber))
        {
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "Thiếu thông tin";
            TempData["ToastMsg"] = "Vui lòng nhập Họ tên và Số điện thoại liên hệ!";
            return RedirectToAction("TestDrive", new { carId = booking.CarId });
        }

        var userEmail = User.Identity?.Name;
        var user = !string.IsNullOrEmpty(userEmail) ? _db.Users.FirstOrDefault(u => u.Email == userEmail) : null;

        var car = booking.CarId.HasValue ? _db.Cars.Find(booking.CarId.Value) : null;

        booking.UserId = user?.Id;
        booking.CarModelName = car != null ? $"{car.Name} ({car.Brand})" : (string.IsNullOrWhiteSpace(booking.CarModelName) ? "Mẫu xe tư vấn" : booking.CarModelName);
        if (booking.PreferredDate < DateTime.Today)
        {
            booking.PreferredDate = DateTime.Today.AddDays(1);
        }
        booking.CreatedAt = DateTime.Now;
        booking.Status = "Chờ xác nhận";

        _db.TestDriveBookings.Add(booking);
        _db.SaveChanges();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Đăng ký thành công!";
        TempData["ToastMsg"] = $"Cảm ơn quý khách {booking.CustomerName}. AutoGarage đã tiếp nhận yêu cầu lái thử xe {booking.CarModelName}. Chuyên viên tư vấn sẽ liên hệ lại sớm nhất!";

        return RedirectToAction("Index");
    }

    // Tiện ích chuyển chuỗi giá tiền sang số nguyên decimal
    private static decimal ParsePriceToNumber(string? priceStr)
    {
        if (string.IsNullOrWhiteSpace(priceStr)) return 0;
        var digits = new string(priceStr.Where(char.IsDigit).ToArray());
        return decimal.TryParse(digits, out var val) ? val : 0;
    }
}
