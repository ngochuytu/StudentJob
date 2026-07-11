namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_NganhNghe")]
public class NganhNghe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdNganhNghe { get; set; }

    [Required(ErrorMessage = "Tên lĩnh vực không được để trống.")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string sTenLinhVuc { get; set; } = null!;

    // Navigation property
    public ICollection<BaiTuyenDung> DsBaiTuyenDung { get; set; } = new List<BaiTuyenDung>();
}
