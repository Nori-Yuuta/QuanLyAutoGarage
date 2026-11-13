namespace AutoGarageMVC.Models.ViewModels;

using AutoGarageMVC.Services;

public class CartVM
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
}
