namespace AutoGarageMVC.Models.Entities;

public class ServiceAppointment
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string LicensePlate { get; set; } = ""; // Biển số xe
    public string CarBrandAndModel { get; set; } = ""; // Ví dụ: Toyota Camry
    public string ServicePackage { get; set; } = "Thay nhớt & Bảo dưỡng định kỳ"; // Thay nhớt, kiểm tra phanh, đồng sơn, bảo dưỡng lớn
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = "08:30 - 10:00";
    public string? CustomerNotes { get; set; }
    public string Status { get; set; } = "Chờ tiếp nhận"; // Chờ tiếp nhận, Đang thực hiện, Đã hoàn thành, Đã hủy
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
