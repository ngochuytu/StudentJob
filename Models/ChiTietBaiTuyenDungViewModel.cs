namespace StudentJob.Models;

public class ChiTietBaiTuyenDungViewModel
{
    public BaiTuyenDung BaiTuyenDung { get; set; } = null!;

    public bool DaDangNhap { get; set; }

    public bool LaSinhVien { get; set; }

    public bool DaUngTuyen { get; set; }

    public bool CoCVMacDinh { get; set; }

    public bool DaHetHan { get; set; }

    public bool DaLuuTin { get; set; }

    public UngTuyenInputModel UngTuyen { get; set; } = new();
}
