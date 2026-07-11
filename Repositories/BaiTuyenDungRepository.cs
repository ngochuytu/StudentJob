namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class BaiTuyenDungRepository : IBaiTuyenDungRepository
{
    private readonly AppDbContext _context;

    public BaiTuyenDungRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<BaiTuyenDung> GetAll()
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .OrderByDescending(b => b.dNgayTaoBai)
            .ToList();
    }

    public BaiTuyenDung? GetById(int id)
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .FirstOrDefault(b => b.PK_IdBaiTuyenDung == id);
    }

    public List<BaiTuyenDung> GetByNhaTuyenDungId(int nhaTuyenDungId)
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .Where(b => b.FK_IdNhaTuyenDung == nhaTuyenDungId)
            .OrderByDescending(b => b.dNgayTaoBai)
            .ToList();
    }

    public List<BaiTuyenDung> GetByCriteria(string? keyword, int? nganhNgheId, int? khuVucId)
    {
        var query = _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .Where(b => b.sTrangThaiKiemDuyet == "Đã duyệt")
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(b =>
                b.sTieuDeCongViec.Contains(keyword) ||
                b.sMoTaCongViec.Contains(keyword));
        }

        if (nganhNgheId.HasValue)
        {
            query = query.Where(b => b.FK_IdNganhNghe == nganhNgheId.Value);
        }

        if (khuVucId.HasValue)
        {
            query = query.Where(b => b.FK_IdKhuVuc == khuVucId.Value);
        }

        return query.OrderByDescending(b => b.dNgayTaoBai).ToList();
    }

    public List<BaiTuyenDung> GetPendingApproval()
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .Where(b => b.sTrangThaiKiemDuyet == "Chờ duyệt")
            .OrderByDescending(b => b.dNgayTaoBai)
            .ToList();
    }

    public void Add(BaiTuyenDung baiTuyenDung)
    {
        _context.DsBaiTuyenDung.Add(baiTuyenDung);
        _context.SaveChanges();
    }

    public void Update(BaiTuyenDung baiTuyenDung)
    {
        _context.DsBaiTuyenDung.Update(baiTuyenDung);
        _context.SaveChanges();
    }
}
