namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;

public class DangKyViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn loại tài khoản.")]
    public string LoaiTaiKhoan { get; set; } = "SinhVien";

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string sEmail { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string sMatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
    [DataType(DataType.Password)]
    [Compare("sMatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhanMatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [StringLength(20)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string sSoDienThoai { get; set; } = null!;

    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string? sHoTen { get; set; }

    [StringLength(100)]
    [Display(Name = "Chuyên ngành học")]
    public string? sChuyenNganhHoc { get; set; }

    [StringLength(150)]
    [Display(Name = "Tên doanh nghiệp")]
    public string? sTenDoanhNghiep { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa chỉ văn phòng")]
    public string? sDiaChiVanPhong { get; set; }
}
