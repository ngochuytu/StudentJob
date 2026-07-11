namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_HoSoUngTuyen")]
public class HoSoUngTuyen
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdHoSoUngTuyen { get; set; }

    [Required]
    [ForeignKey(nameof(SinhVien))]
    public int FK_IdSinhVien { get; set; }

    [Required]
    [ForeignKey(nameof(BaiTuyenDung))]
    public int FK_IdBaiTuyenDung { get; set; }

    [Required(ErrorMessage = "Đường dẫn CV không được để trống.")]
    [StringLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string sDuongDanCV { get; set; } = null!;

    [Column(TypeName = "nvarchar(max)")]
    public string? sThuXinViec { get; set; }

    [Required(ErrorMessage = "Trạng thái xét duyệt không được để trống.")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string sTrangThaiXetDuyet { get; set; } = "Chờ duyệt";

    [StringLength(200)]
    [Column(TypeName = "nvarchar(200)")]
    public string? sGhiChuPhanHoi { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime dThoiGianNopHoSo { get; set; }

    // Navigation properties
    public SinhVien SinhVien { get; set; } = null!;
    public BaiTuyenDung BaiTuyenDung { get; set; } = null!;
}
