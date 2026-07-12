namespace StudentJob.Repositories;

using StudentJob.Models;

public interface IHoSoUngTuyenRepository
{
    List<HoSoUngTuyen> GetBySinhVienId(int sinhVienId);
    List<HoSoUngTuyen> GetByBaiTuyenDungId(int baiTuyenDungId);
    List<HoSoUngTuyen> GetByNhaTuyenDungId(int nhaTuyenDungId);
    HoSoUngTuyen? GetById(int id);

    bool HasApplied(int sinhVienId, int baiTuyenDungId);
    bool HasApplicationsForJob(int baiTuyenDungId);

    void Add(HoSoUngTuyen hoSoUngTuyen);
    void Update(HoSoUngTuyen hoSoUngTuyen);
    List<HoSoUngTuyen> GetAll();
}
