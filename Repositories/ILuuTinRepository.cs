namespace StudentJob.Repositories;

using StudentJob.Models;

public interface ILuuTinRepository
{
    List<LuuTin> GetBySinhVienId(int sinhVienId);

    bool IsSaved(int sinhVienId, int baiTuyenDungId);

    LuuTin? Get(int sinhVienId, int baiTuyenDungId);

    void Add(LuuTin luuTin);

    void Delete(LuuTin luuTin);
}
