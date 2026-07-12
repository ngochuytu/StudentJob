namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class LuuTinRepository : ILuuTinRepository
{
    private readonly AppDbContext _context;

    public LuuTinRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<LuuTin> GetBySinhVienId(int sinhVienId)
    {
        return _context.DsLuuTin
            .Include(l => l.BaiTuyenDung)
                .ThenInclude(b => b.NhaTuyenDung)
            .Include(l => l.BaiTuyenDung)
                .ThenInclude(b => b.NganhNghe)
            .Include(l => l.BaiTuyenDung)
                .ThenInclude(b => b.KhuVuc)
            .Where(l => l.FK_IdSinhVien == sinhVienId)
            .OrderByDescending(l => l.dNgayLuu)
            .ToList();
    }

    public bool IsSaved(int sinhVienId, int baiTuyenDungId)
    {
        return _context.DsLuuTin.Any(l =>
            l.FK_IdSinhVien == sinhVienId &&
            l.FK_IdBaiTuyenDung == baiTuyenDungId);
    }

    public LuuTin? Get(int sinhVienId, int baiTuyenDungId)
    {
        return _context.DsLuuTin.FirstOrDefault(l =>
            l.FK_IdSinhVien == sinhVienId &&
            l.FK_IdBaiTuyenDung == baiTuyenDungId);
    }

    public void Add(LuuTin luuTin)
    {
        _context.DsLuuTin.Add(luuTin);
        _context.SaveChanges();
    }

    public void Delete(LuuTin luuTin)
    {
        _context.DsLuuTin.Remove(luuTin);
        _context.SaveChanges();
    }
}