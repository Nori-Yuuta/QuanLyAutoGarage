namespace AutoGarageMVC.Models.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Đang diễn ra";
}
