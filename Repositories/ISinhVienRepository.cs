namespace StudentJob.Repositories;

using StudentJob.Models;

public interface ISinhVienRepository
{
    SinhVien? GetByTaiKhoanId(int taiKhoanId);
    SinhVien? GetById(int id);
    void Add(SinhVien sinhVien);
    void Update(SinhVien sinhVien);
}
