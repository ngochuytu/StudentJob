namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_NhaTuyenDung")]
public class NhaTuyenDung
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdNhaTuyenDung { get; set; }

    [Required]
    [ForeignKey(nameof(TaiKhoan))]
    public int FK_IdTaiKhoan { get; set; }

    [Required(ErrorMessage = "Tên doanh nghiệp không được để trống.")]
    [StringLength(150)]
    [Column(TypeName = "nvarchar(150)")]
    public string sTenDoanhNghiep { get; set; } = null!;

    [StringLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string? sDuongDanAnhLogo { get; set; }

    [Required(ErrorMessage = "Địa chỉ văn phòng không được để trống.")]
    [StringLength(200)]
    [Column(TypeName = "nvarchar(200)")]
    public string sDiaChiVanPhong { get; set; } = null!;

    [Column(TypeName = "nvarchar(max)")]
    public string? sMoTaTongQuan { get; set; }

    // Navigation properties
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<BaiTuyenDung> DsBaiTuyenDung { get; set; } = new List<BaiTuyenDung>();
}
