namespace AutoGarageMVC.Models.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = "";  // "ORD123..."
    public string UserEmail { get; set; } = "";
    public string RecipientName { get; set; } = "";
    public string RecipientPhone { get; set; } = "";
    public string? RecipientEmail { get; set; }
    public string Province { get; set; } = "";
    public string District { get; set; } = "";
    public string Address { get; set; } = "";
    public string? Note { get; set; }
    public string Payment { get; set; } = "cod";
    public string? Coupon { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();
}
