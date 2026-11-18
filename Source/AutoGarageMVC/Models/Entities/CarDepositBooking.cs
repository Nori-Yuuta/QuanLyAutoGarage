namespace AutoGarageMVC.Models.Entities;

public class CarDepositBooking
{
    public int Id { get; set; }
    public string DepositCode { get; set; } = ""; // Mã cọc: DCxxxxxx
    public int? CarId { get; set; }
    public virtual Car? Car { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Email { get; set; }
    public string CarName { get; set; } = "";
    public string? CarColor { get; set; }
    public decimal DepositAmount { get; set; } = 20_000_000; // Tiền đặt cọc (mặc định 20 triệu hoặc tùy chọn)
    public string PaymentMethod { get; set; } = "QR_TRANSFER"; // QR_TRANSFER, VNPAY, MOMO
    public string PaymentStatus { get; set; } = "Chờ thanh toán"; // Chờ thanh toán, Đã thanh toán, Đã xác nhận giữ xe, Đã hoàn cọc
    public string? TransactionRef { get; set; } // Mã giao dịch ngân hàng / cổng TT
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
