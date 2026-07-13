namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class UngTuyenInputModel
{
    [Required]
    public int FK_IdBaiTuyenDung { get; set; }

    [Display(Name = "CV ứng tuyển")]
    public IFormFile? TepCV { get; set; }

    [StringLength(3000, ErrorMessage = "Thư xin việc không được vượt quá 3000 ký tự.")]
    [Display(Name = "Thư xin việc")]
    public string? sThuXinViec { get; set; }

    public bool SuDungCVMacDinh { get; set; }
}
