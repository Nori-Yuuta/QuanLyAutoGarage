namespace AutoGarageMVC.Models.Entities;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Price { get; set; } = "";
    public string? ImageUrl { get; set; }
    public int Year { get; set; }
    public string? Engine { get; set; }
    public string? Fuel { get; set; }
    public int? Seats { get; set; }
    public string? Transmission { get; set; }
    public string? Origin { get; set; }
    public string? Description { get; set; }
}
