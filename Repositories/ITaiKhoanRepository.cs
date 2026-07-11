namespace StudentJob.Repositories;

using StudentJob.Models;

public interface ITaiKhoanRepository
{
    TaiKhoan? GetByEmail(string email);
    TaiKhoan? GetById(int id);
    List<TaiKhoan> GetAll();
    void Add(TaiKhoan taiKhoan);
    void Update(TaiKhoan taiKhoan);
    void UpdateTrangThai(int id, bool active);
}
