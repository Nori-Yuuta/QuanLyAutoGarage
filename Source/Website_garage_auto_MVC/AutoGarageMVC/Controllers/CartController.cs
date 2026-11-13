namespace AutoGarageMVC.Controllers;

using AutoGarageMVC.Models.ViewModels;
using AutoGarageMVC.Services;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart) => _cart = cart;

    public IActionResult Index()
    {
        var items = _cart.Get();
        var vm = new CartVM
        {
            Items = items,
            Subtotal = _cart.Subtotal,
            Shipping = _cart.Shipping,
            Total = _cart.Subtotal + _cart.Shipping
        };

        ViewBag.Subtotal = vm.Subtotal;
        ViewBag.Shipping = vm.Shipping;
        ViewBag.Total = vm.Total;

        return View(items);
    }

    [HttpPost]
    public IActionResult UpdateQty(int id, int delta)
    {
        var item = _cart.Get().FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            var newQty = item.Qty + delta;
            if (newQty > 0)
                _cart.UpdateQty(id, newQty);
            else
                _cart.Remove(id);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Increase(int id)
    {
        var item = _cart.Get().FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            _cart.UpdateQty(id, item.Qty + 1);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Decrease(int id)
    {
        var item = _cart.Get().FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            if (item.Qty > 1)
            {
                _cart.UpdateQty(id, item.Qty - 1);
            }
            else
            {
                _cart.Remove(id);
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int id)
    {
        _cart.Remove(id);
        TempData["ToastType"] = "info";
        TempData["ToastTitle"] = "Đã xóa";
        TempData["ToastMsg"] = "Đã xóa sản phẩm khỏi giỏ hàng";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Clear()
    {
        _cart.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Checkout" });

        return RedirectToAction("Index", "Checkout");
    }
}
