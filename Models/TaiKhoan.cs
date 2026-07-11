namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_TaiKhoan")]
public class TaiKhoan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdTaiKhoan { get; set; }

    [Required(ErrorMessage = "Email không được để trống.")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Column(TypeName = "varchar(100)")]
    public string sEmail { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [StringLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string sMatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [StringLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string sSoDienThoai { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(VaiTro))]
    public int FK_IdVaiTro { get; set; }

    [Required]
    public bool bTrangThaiHoatDong { get; set; } = true;

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime dNgayTaoTaiKhoan { get; set; }

    // Navigation properties
    public VaiTro VaiTro { get; set; } = null!;
    public SinhVien? SinhVien { get; set; }
    public NhaTuyenDung? NhaTuyenDung { get; set; }
}
