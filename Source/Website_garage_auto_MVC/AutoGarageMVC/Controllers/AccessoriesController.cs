namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;

public class AccessoriesController : Controller
{
    private readonly AutoGarageDbContext _db;

    public AccessoriesController(AutoGarageDbContext db)
    {
        _db = db;
    }

    public IActionResult Index(string? category, string? search)
    {
        var q = _db.Accessories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            q = q.Where(a => a.Category.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(a => a.Name.ToLower().Contains(search.ToLower()) || a.Category.ToLower().Contains(search.ToLower()));
        }

        ViewBag.Category = category;
        ViewBag.Search = search;
        ViewBag.AllCategories = new[] { "Nội thất", "Ngoại thất", "An toàn", "Vận hành" };

        return View(q.ToList());
    }
}
