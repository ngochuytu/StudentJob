namespace StudentJob.Repositories;

using StudentJob.Models;

public interface IBaiTuyenDungRepository
{
    List<BaiTuyenDung> GetAll();
    BaiTuyenDung? GetById(int id);
    List<BaiTuyenDung> GetByNhaTuyenDungId(int nhaTuyenDungId);
    List<BaiTuyenDung> GetByCriteria(string? keyword, int? nganhNgheId, int? khuVucId);
    List<BaiTuyenDung> GetPendingApproval();
    void Add(BaiTuyenDung baiTuyenDung);
    void Update(BaiTuyenDung baiTuyenDung);
}
