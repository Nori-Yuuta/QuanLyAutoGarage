namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly AutoGarageDbContext _db;

    public HomeController(AutoGarageDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        ViewBag.Cars = _db.Cars.Take(6).ToList();
        ViewBag.Accessories = _db.Accessories.Take(6).ToList();
        ViewBag.ActiveEvents = _db.Events.Where(e => e.Status == "Đang diễn ra").ToList();
        return View();
    }

    public IActionResult About()
    {
        ViewBag.Title = "Giới Thiệu";
        return View();
    }

    public IActionResult News()
    {
        ViewBag.Title = "Tin Tức";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
