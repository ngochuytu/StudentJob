namespace StudentJob.Repositories;

using StudentJob.Models;

public interface INhaTuyenDungRepository
{
    NhaTuyenDung? GetByTaiKhoanId(int taiKhoanId);
    NhaTuyenDung? GetById(int id);
    void Add(NhaTuyenDung nhaTuyenDung);
    void Update(NhaTuyenDung nhaTuyenDung);
}
