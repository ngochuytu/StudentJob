namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;

public class CapNhatTrangThaiHoSoInputModel
{
    [Required]
    public int PK_IdHoSoUngTuyen { get; set; }

    [Required(ErrorMessage = "Trạng thái hồ sơ không được để trống.")]
    public string sTrangThaiXetDuyet { get; set; } = null!;

    [StringLength(200, ErrorMessage = "Ghi chú phản hồi không được vượt quá 200 ký tự.")]
    public string? sGhiChuPhanHoi { get; set; }
}
