namespace StudentJob.Repositories;

using StudentJob.Models;

public interface IBaiTuyenDungRepository
{
    List<BaiTuyenDung> GetAll();

    List<BaiTuyenDung> GetApprovedJobs();

    List<BaiTuyenDung> GetByNhaTuyenDungId(int nhaTuyenDungId);

    BaiTuyenDung? GetById(int id);

    BaiTuyenDung? GetApprovedById(int id);

    List<BaiTuyenDung> GetByDieuKien(
        string? keyword,
        string? hinhThuc,
        int? nganhNgheId,
        int? khuVucId
    );

    void Add(BaiTuyenDung baiTuyenDung);

    void Update(BaiTuyenDung baiTuyenDung);

    void Delete(BaiTuyenDung baiTuyenDung);
}
