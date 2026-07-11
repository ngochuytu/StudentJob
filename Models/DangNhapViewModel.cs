namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;

public class DangNhapViewModel
{
    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string sEmail { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string sMatKhau { get; set; } = null!;
}
