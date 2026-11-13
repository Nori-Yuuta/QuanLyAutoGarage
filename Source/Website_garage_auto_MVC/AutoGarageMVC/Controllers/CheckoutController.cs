namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models;
using AutoGarageMVC.Models.Entities;
using AutoGarageMVC.Models.ViewModels;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CheckoutController : Controller
{
    private readonly AutoGarageDbContext _db;
    private readonly CartService _cart;

    public CheckoutController(AutoGarageDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var items = _cart.Get();
        if (!items.Any()) return RedirectToAction("Index", "Cart");

        var userEmail = User.Identity?.Name;
        var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);

        ViewBag.Items = items;
        ViewBag.Subtotal = _cart.Subtotal;
        ViewBag.Shipping = _cart.Shipping;
        ViewBag.Total = _cart.Subtotal + _cart.Shipping;

        return View(new CheckoutVM
        {
            Name = user?.Name ?? "",
            Phone = user?.Phone ?? "",
            Email = user?.Email ?? ""
        });
    }

    [HttpPost]
    public IActionResult Index(CheckoutVM vm)
    {
        var items = _cart.Get();
        if (!items.Any()) return RedirectToAction("Index", "Cart");

        if (!ModelState.IsValid)
        {
            ViewBag.Items = items;
            ViewBag.Subtotal = _cart.Subtotal;
            ViewBag.Shipping = _cart.Shipping;
            ViewBag.Total = _cart.Subtotal + _cart.Shipping;
            return View(vm);
        }

        var subtotal = _cart.Subtotal;
        var shipping = _cart.Shipping;
        decimal discount = 0;
        var coupon = vm.Coupon?.Trim().ToUpper();

        if (coupon == "SALE30")
        {
            discount = Math.Min(subtotal * 0.3m, 500_000m);
        }
        else if (coupon == "WELCOME10")
        {
            discount = Math.Min(subtotal * 0.1m, 200_000m);
        }
        else if (coupon == "SHIPFREE")
        {
            shipping = 0;
        }

        var order = new Order
        {
            OrderCode = "ORD" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UserEmail = User.Identity?.Name ?? vm.Email ?? "guest@autogarage.vn",
            RecipientName = vm.Name,
            RecipientPhone = vm.Phone,
            RecipientEmail = vm.Email,
            Province = vm.Province,
            District = vm.District,
            Address = vm.Address,
            Note = vm.Note,
            Payment = vm.Payment,
            Coupon = coupon,
            Subtotal = subtotal,
            Shipping = shipping,
            Discount = discount,
            Total = Math.Max(0, subtotal + shipping - discount),
            Items = items.Select(i => new OrderItem
            {
                AccessoryId = i.Id,
                Name = i.Name,
                ImageUrl = i.ImageUrl,
                Category = i.Category,
                Price = i.Price,
                Qty = i.Qty
            }).ToList()
        };

        _db.Orders.Add(order);
        _db.SaveChanges();
        _cart.Clear();

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Đặt hàng thành công!";
        TempData["ToastMsg"] = $"Mã đơn: {order.OrderCode} — Tổng: {order.Total:N0} ₫";

        return RedirectToAction("Index", "Profile");
    }
}
