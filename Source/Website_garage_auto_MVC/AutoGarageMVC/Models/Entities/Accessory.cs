namespace AutoGarageMVC.Models.Entities;

public class Accessory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";   // Nội thất | Ngoại thất | An toàn | Vận hành
    public string Price { get; set; } = "";
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public string? Material { get; set; }
    public string? Warranty { get; set; }
    public string? Compat { get; set; }          // lưu dạng "Toyota Camry,Honda CR-V,..."
}
