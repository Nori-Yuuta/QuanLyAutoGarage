namespace AutoGarageMVC.Models.Entities;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = "Tin tức"; // Xe Mới | Kinh Nghiệm | Công Nghệ | Bảo Dưỡng
    public int? AuthorId { get; set; }
    public User? Author { get; set; }
    public int Views { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
