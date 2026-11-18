namespace AutoGarageMVC.Models.ViewModels;

using AutoGarageMVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

public class ProfileVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = "";

    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }

    public List<Order> Orders { get; set; } = new();
}
