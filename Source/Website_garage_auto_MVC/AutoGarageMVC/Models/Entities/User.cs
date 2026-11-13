namespace AutoGarageMVC.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";      // unique
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "customer"; // customer | admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
