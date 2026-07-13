namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_LuuTin")]
public class LuuTin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdLuuTin { get; set; }

    [Required]
    [ForeignKey(nameof(SinhVien))]
    public int FK_IdSinhVien { get; set; }

    [Required]
    [ForeignKey(nameof(BaiTuyenDung))]
    public int FK_IdBaiTuyenDung { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime dNgayLuu { get; set; }

    public SinhVien SinhVien { get; set; } = null!;

    public BaiTuyenDung BaiTuyenDung { get; set; } = null!;
}
