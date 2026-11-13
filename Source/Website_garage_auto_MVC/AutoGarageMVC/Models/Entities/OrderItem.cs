namespace AutoGarageMVC.Models.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int AccessoryId { get; set; }
    public string Name { get; set; } = "";
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
}
