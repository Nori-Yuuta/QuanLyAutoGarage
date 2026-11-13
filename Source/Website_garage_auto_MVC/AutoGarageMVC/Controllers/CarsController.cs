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

    public IActionResult Index(string? brand, string? search)
    {
        var query = _db.Cars.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(c => c.Brand.ToLower() == brand.ToLower() || c.Brand.ToLower().Contains(brand.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()) || c.Brand.ToLower().Contains(search.ToLower()));
        }

        ViewBag.Brand = brand;
        ViewBag.Search = search;
        ViewBag.AllBrands = new[] { "toyota", "honda", "ford", "mazda", "bmw", "mercedes" };

        return View(query.ToList());
    }
}
