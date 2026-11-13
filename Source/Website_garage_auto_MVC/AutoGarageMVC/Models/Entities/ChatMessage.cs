namespace AutoGarageMVC.Models.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string From { get; set; } = "";       // "customer" | "admin"
    public string Sender { get; set; } = "";
    public string? SenderEmail { get; set; }
    public string Text { get; set; } = "";
    public DateTime Time { get; set; } = DateTime.UtcNow;
}
