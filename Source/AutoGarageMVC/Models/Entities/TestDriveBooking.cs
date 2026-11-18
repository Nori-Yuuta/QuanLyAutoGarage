namespace AutoGarageMVC.Models.Entities;

public class TestDriveBooking
{
    public int Id { get; set; }
    public int? CarId { get; set; }
    public virtual Car? Car { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Email { get; set; }
    public string CarModelName { get; set; } = "";
    public DateTime PreferredDate { get; set; }
    public string PreferredTimeSlot { get; set; } = "Sáng (08:30 - 11:30)"; // Sáng, Chiều
    public string LocationType { get; set; } = "Showroom"; // Showroom, Tại nhà
    public string? HomeAddress { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = "Chờ xác nhận"; // Chờ xác nhận, Đã xác nhận, Hoàn thành, Đã hủy
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
