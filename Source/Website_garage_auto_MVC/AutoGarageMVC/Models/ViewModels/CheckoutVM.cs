namespace AutoGarageMVC.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class CheckoutVM
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên người nhận")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
    public string Phone { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh / Thành phố")]
    public string Province { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập Quận / Huyện")]
    public string District { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết")]
    public string Address { get; set; } = "";

    public string? Note { get; set; }
    public string Payment { get; set; } = "cod";
    public string? Coupon { get; set; }
}
