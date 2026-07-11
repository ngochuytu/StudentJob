namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_BaiTuyenDung")]
public class BaiTuyenDung
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdBaiTuyenDung { get; set; }

    [Required]
    [ForeignKey(nameof(NhaTuyenDung))]
    public int FK_IdNhaTuyenDung { get; set; }

    [Required(ErrorMessage = "Tiêu đề công việc không được để trống.")]
    [StringLength(150)]
    [Column(TypeName = "nvarchar(150)")]
    public string sTieuDeCongViec { get; set; } = null!;

    [Required(ErrorMessage = "Hình thức làm việc không được để trống.")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string sHinhThucLamViec { get; set; } = null!;

    [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
    [Column(TypeName = "nvarchar(max)")]
    public string sMoTaCongViec { get; set; } = null!;

    [StringLength(200)]
    [Column(TypeName = "nvarchar(200)")]
    public string? sCaLam { get; set; }

    [Required(ErrorMessage = "Mức lương không được để trống.")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string sMucLuong { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(NganhNghe))]
    public int FK_IdNganhNghe { get; set; }

    [Required]
    [ForeignKey(nameof(KhuVuc))]
    public int FK_IdKhuVuc { get; set; }

    [Required(ErrorMessage = "Hạn nộp hồ sơ không được để trống.")]
    [Column(TypeName = "date")]
    public DateOnly dHanNopHoSo { get; set; }

    [Required(ErrorMessage = "Trạng thái kiểm duyệt không được để trống.")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string sTrangThaiKiemDuyet { get; set; } = "Chờ duyệt";

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime dNgayTaoBai { get; set; }

    // Navigation properties
    public NhaTuyenDung NhaTuyenDung { get; set; } = null!;
    public NganhNghe NganhNghe { get; set; } = null!;
    public KhuVuc KhuVuc { get; set; } = null!;
    public ICollection<HoSoUngTuyen> DsHoSoUngTuyen { get; set; } = new List<HoSoUngTuyen>();
}
