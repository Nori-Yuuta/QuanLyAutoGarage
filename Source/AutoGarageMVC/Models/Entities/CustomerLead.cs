namespace AutoGarageMVC.Models.Entities;

public class CustomerLead
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Email { get; set; }
    public string LeadSource { get; set; } = "Lái thử"; // Lái thử, Báo giá, Thu cũ đổi mới, Chat AI, Đặt cọc
    public string InterestedCarOrService { get; set; } = "";
    public string? AssignedConsultant { get; set; } = "Chưa phân công"; // Nhân viên sales phụ trách
    public string Stage { get; set; } = "Mới tiếp nhận"; // Mới tiếp nhận, Đang liên hệ, Hẹn xem xe, Đàm phán, Đã chốt, Hủy
    public string? Notes { get; set; }
    public decimal? ExpectedValue { get; set; } // Giá trị tiềm năng
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
