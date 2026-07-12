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

    public List<BaiTuyenDung> GetApprovedJobs()
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .Where(b =>
                b.sTrangThaiKiemDuyet == "Đã duyệt" &&
                b.dHanNopHoSo >= DateOnly.FromDateTime(DateTime.Today))
            .OrderByDescending(b => b.dNgayTaoBai)
            .ToList();
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

    public BaiTuyenDung? GetById(int id)
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .FirstOrDefault(b => b.PK_IdBaiTuyenDung == id);
    }

    public BaiTuyenDung? GetApprovedById(int id)
    {
        return _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .FirstOrDefault(b =>
                b.PK_IdBaiTuyenDung == id &&
                b.sTrangThaiKiemDuyet == "Đã duyệt");
    }

    public List<BaiTuyenDung> GetByDieuKien(
        string? keyword,
        string? hinhThuc,
        int? nganhNgheId,
        int? khuVucId)
    {
        var query = _context.DsBaiTuyenDung
            .Include(b => b.NhaTuyenDung)
            .Include(b => b.NganhNghe)
            .Include(b => b.KhuVuc)
            .Where(b =>
                b.sTrangThaiKiemDuyet == "Đã duyệt" &&
                b.dHanNopHoSo >= DateOnly.FromDateTime(DateTime.Today))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string trimmedKeyword = keyword.Trim();

            query = query.Where(b =>
                b.sTieuDeCongViec.Contains(trimmedKeyword) ||
                b.sMoTaCongViec.Contains(trimmedKeyword) ||
                b.NhaTuyenDung.sTenDoanhNghiep.Contains(trimmedKeyword));
        }

        if (!string.IsNullOrWhiteSpace(hinhThuc))
        {
            query = query.Where(
                b => b.sHinhThucLamViec == hinhThuc);
        }

        if (nganhNgheId.HasValue)
        {
            query = query.Where(
                b => b.FK_IdNganhNghe == nganhNgheId.Value);
        }

        if (khuVucId.HasValue)
        {
            query = query.Where(
                b => b.FK_IdKhuVuc == khuVucId.Value);
        }

        return query
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

    public void Delete(BaiTuyenDung baiTuyenDung)
    {
        _context.DsBaiTuyenDung.Remove(baiTuyenDung);
        _context.SaveChanges();
    }
}