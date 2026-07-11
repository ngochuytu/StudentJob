namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class HoSoUngTuyenRepository : IHoSoUngTuyenRepository
{
    private readonly AppDbContext _context;

    public HoSoUngTuyenRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<HoSoUngTuyen> GetBySinhVienId(int sinhVienId)
    {
        return _context.DsHoSoUngTuyen
            .Include(h => h.BaiTuyenDung)
                .ThenInclude(b => b.NhaTuyenDung)
            .Where(h => h.FK_IdSinhVien == sinhVienId)
            .OrderByDescending(h => h.dThoiGianNopHoSo)
            .ToList();
    }

    public List<HoSoUngTuyen> GetByBaiTuyenDungId(int baiTuyenDungId)
    {
        return _context.DsHoSoUngTuyen
            .Include(h => h.SinhVien)
            .Where(h => h.FK_IdBaiTuyenDung == baiTuyenDungId)
            .OrderByDescending(h => h.dThoiGianNopHoSo)
            .ToList();
    }

    public HoSoUngTuyen? GetById(int id)
    {
        return _context.DsHoSoUngTuyen
            .Include(h => h.SinhVien)
            .Include(h => h.BaiTuyenDung)
                .ThenInclude(b => b.NhaTuyenDung)
            .FirstOrDefault(h => h.PK_IdHoSoUngTuyen == id);
    }

    public bool HasApplied(int sinhVienId, int baiTuyenDungId)
    {
        return _context.DsHoSoUngTuyen
            .Any(h => h.FK_IdSinhVien == sinhVienId && h.FK_IdBaiTuyenDung == baiTuyenDungId);
    }

    public void Add(HoSoUngTuyen hoSoUngTuyen)
    {
        _context.DsHoSoUngTuyen.Add(hoSoUngTuyen);
        _context.SaveChanges();
    }

    public void Update(HoSoUngTuyen hoSoUngTuyen)
    {
        _context.DsHoSoUngTuyen.Update(hoSoUngTuyen);
        _context.SaveChanges();
    }

    public List<HoSoUngTuyen> GetAll()
    {
        return _context.DsHoSoUngTuyen.ToList();
    }
}
