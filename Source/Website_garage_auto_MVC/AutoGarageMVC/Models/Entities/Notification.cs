namespace AutoGarageMVC.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
