namespace StudentJob.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_VaiTro")]
public class VaiTro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PK_IdVaiTro { get; set; }

    [Required(ErrorMessage = "Tên vai trò không được để trống.")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string sTenVaiTro { get; set; } = null!;

    // Navigation property
    public ICollection<TaiKhoan> DsTaiKhoan { get; set; } = new List<TaiKhoan>();
}
