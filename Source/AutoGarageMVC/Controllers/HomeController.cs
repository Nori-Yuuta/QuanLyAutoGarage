namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        ViewBag.LatestNews = _db.News.OrderByDescending(n => n.CreatedAt).Take(3).ToList();
        return View();
    }

    public IActionResult About()
    {
        ViewBag.Title = "Giới Thiệu";
        return View();
    }

    public IActionResult News(string? category, string? search)
    {
        ViewBag.Title = "Bản Tin & Kiến Thức Ô Tô";
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        var query = _db.News.Include(n => n.Author).AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(n => n.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(n => n.Title.ToLower().Contains(s) || n.Summary.ToLower().Contains(s));
        }

        var newsList = query.OrderByDescending(n => n.CreatedAt).ToList();
        return View(newsList);
    }

    public IActionResult NewsDetail(int id)
    {
        var news = _db.News.Include(n => n.Author).FirstOrDefault(n => n.Id == id);
        if (news == null)
        {
            return NotFound();
        }

        // Tăng lượt xem
        news.Views++;
        _db.SaveChanges();

        ViewBag.Title = news.Title;
        ViewBag.RelatedNews = _db.News
            .Where(n => n.Id != id)
            .OrderByDescending(n => n.CreatedAt)
            .Take(3)
            .ToList();

        return View(news);
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
