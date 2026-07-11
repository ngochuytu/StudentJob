namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_SinhVien")]
public class SinhVien
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdSinhVien { get; set; }

    [Required]
    [ForeignKey(nameof(TaiKhoan))]
    public int FK_IdTaiKhoan { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống.")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string sHoTen { get; set; } = null!;

    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? sChuyenNganhHoc { get; set; }

    [StringLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string? sDuongDanCVMacDinh { get; set; }

    // Navigation properties
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<HoSoUngTuyen> DsHoSoUngTuyen { get; set; } = new List<HoSoUngTuyen>();
}
