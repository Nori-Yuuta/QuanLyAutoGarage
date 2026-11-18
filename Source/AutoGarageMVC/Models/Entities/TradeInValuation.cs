namespace AutoGarageMVC.Models.Entities;

public class TradeInValuation
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Email { get; set; }

    // Thông tin xe cũ
    public string CurrentBrand { get; set; } = "";
    public string CurrentModel { get; set; } = "";
    public int ManufactureYear { get; set; }
    public string Transmission { get; set; } = "Tự động";
    public int MileageKm { get; set; } // Số km đã đi (ODO)
    public string? ImageUrl { get; set; } // Ảnh xe cũ tải lên
    public string? ConditionDescription { get; set; } // Tình trạng xe

    // Mẫu xe mới muốn đổi sang (nếu có)
    public string? TargetCarModel { get; set; }

    public decimal? EstimatedPrice { get; set; } // Showroom định giá dự kiến
    public string Status { get; set; } = "Chờ thẩm định"; // Chờ thẩm định, Đã gửi định giá, Đã chốt thu xe, Hủy
    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
