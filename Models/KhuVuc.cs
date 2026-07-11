namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_KhuVuc")]
public class KhuVuc
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdKhuVuc { get; set; }

    [Required(ErrorMessage = "Tên khu vực không được để trống.")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string sTenKhuVuc { get; set; } = null!;

    // Navigation property
    public ICollection<BaiTuyenDung> DsBaiTuyenDung { get; set; } = new List<BaiTuyenDung>();
}
