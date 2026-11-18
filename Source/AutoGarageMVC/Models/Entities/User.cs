namespace AutoGarageMVC.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";      // unique
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "customer"; // customer | admin
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public List<Order> Orders { get; set; } = new();
    public List<ChatMessage> ChatMessages { get; set; } = new();
    public List<News> NewsArticles { get; set; } = new();
    public List<Notification> Notifications { get; set; } = new();
}
