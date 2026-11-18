namespace AutoGarageMVC.Models.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string SessionId { get; set; } = "";  // Mã định danh phiên chat riêng của từng khách
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string From { get; set; } = "";       // "customer" | "ai" | "admin"
    public string Sender { get; set; } = "AutoGarage AI";
    public string? SenderEmail { get; set; }
    public string Text { get; set; } = "";
    public DateTime Time { get; set; } = DateTime.Now;
}
