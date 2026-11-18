namespace AutoGarageMVC.Models.Entities;

public class ServiceHistoryRecord
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = ""; // Biển số xe
    public string? VinNumber { get; set; } // Mã khung (VIN)
    public string CarModel { get; set; } = ""; // Dòng xe
    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";

    public DateTime ServiceDate { get; set; } = DateTime.Now;
    public int MileageKm { get; set; } // ODO lúc bảo dưỡng
    public string ServicePerformed { get; set; } = ""; // Hạng mục đã thực hiện
    public string? PartsReplaced { get; set; } // Phụ tùng thay thế
    public decimal TotalCost { get; set; } // Chi phí sửa chữa/bảo dưỡng
    public string TechnicianName { get; set; } = "KTV AutoGarage";
    public string? NextServiceRecommendation { get; set; } // Hẹn đợt bảo dưỡng kế tiếp
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
