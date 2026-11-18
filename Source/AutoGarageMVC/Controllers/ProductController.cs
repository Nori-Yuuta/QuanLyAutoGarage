namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly CartService _cart;

    public ProductController(AutoGarageDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    public IActionResult Detail(string type, int id)
    {
        if (type == "car")
        {
            var car = _db.Cars.Find(id);
            if (car == null) return NotFound();

            ViewBag.Type = "car";
            ViewBag.Car = car;

            var brandPrefix = car.Brand.ToLower();
            ViewBag.Compat = _db.Accessories
                .Where(a => a.Compat != null && a.Compat.ToLower().Contains(brandPrefix))
                .ToList();

            return View("Detail", car);
        }

        if (type == "acc")
        {
            var acc = _db.Accessories.Find(id);
            if (acc == null) return NotFound();

            ViewBag.Type = "acc";
            ViewBag.Accessory = acc;

            return View("Detail", acc);
        }

        return BadRequest("Loại sản phẩm không hợp lệ");
    }

    [HttpPost]
    public IActionResult AddToCart(int id, int qty = 1)
    {
        var acc = _db.Accessories.Find(id);
        if (acc == null) return NotFound();

        var cleanPrice = System.Text.RegularExpressions.Regex.Replace(acc.Price, @"[^\d]", "");
        decimal price = decimal.TryParse(cleanPrice, out var p) ? p : 0;

        _cart.Add(new CartItem(
            acc.Id,
            acc.Name,
            price,
            acc.ImageUrl,
            acc.Category,
            Math.Max(1, qty)
        ));

        TempData["ToastType"] = "cart";
        TempData["ToastTitle"] = "Đã thêm vào giỏ!";
        TempData["ToastMsg"] = $"{Math.Max(1, qty)} × {acc.Name}";

        return RedirectToAction("Index", "Cart");
    }
}
