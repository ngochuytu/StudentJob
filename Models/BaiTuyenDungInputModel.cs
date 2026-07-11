namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;

public class BaiTuyenDungInputModel
{
    [Required(ErrorMessage = "Tiêu đề công việc không được để trống.")]
    [StringLength(150, ErrorMessage = "Tiêu đề công việc không được vượt quá 150 ký tự.")]
    [Display(Name = "Tiêu đề công việc")]
    public string sTieuDeCongViec { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn hình thức làm việc.")]
    [StringLength(50)]
    [Display(Name = "Hình thức làm việc")]
    public string sHinhThucLamViec { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
    [Display(Name = "Mô tả công việc")]
    public string sMoTaCongViec { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Ca làm không được vượt quá 200 ký tự.")]
    [Display(Name = "Ca làm")]
    public string? sCaLam { get; set; }

    [Required(ErrorMessage = "Mức lương không được để trống.")]
    [StringLength(50, ErrorMessage = "Mức lương không được vượt quá 50 ký tự.")]
    [Display(Name = "Mức lương")]
    public string sMucLuong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn ngành nghề.")]
    [Display(Name = "Ngành nghề")]
    public int? FK_IdNganhNghe { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khu vực.")]
    [Display(Name = "Khu vực")]
    public int? FK_IdKhuVuc { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn hạn nộp hồ sơ.")]
    [DataType(DataType.Date)]
    [Display(Name = "Hạn nộp hồ sơ")]
    public DateTime? dHanNopHoSo { get; set; }
}
