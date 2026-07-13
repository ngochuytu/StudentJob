namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class NhaTuyenDungRepository : INhaTuyenDungRepository
{
    private readonly AppDbContext _context;

    public NhaTuyenDungRepository(AppDbContext context)
    {
        _context = context;
    }

    public NhaTuyenDung? GetByTaiKhoanId(int taiKhoanId)
    {
        return _context
            .DsNhaTuyenDung.Include(n => n.TaiKhoan)
            .FirstOrDefault(n => n.FK_IdTaiKhoan == taiKhoanId);
    }

    public NhaTuyenDung? GetById(int id)
    {
        return _context
            .DsNhaTuyenDung.Include(n => n.TaiKhoan)
            .FirstOrDefault(n => n.PK_IdNhaTuyenDung == id);
    }

    public void Add(NhaTuyenDung nhaTuyenDung)
    {
        _context.DsNhaTuyenDung.Add(nhaTuyenDung);
        _context.SaveChanges();
    }

    public void Update(NhaTuyenDung nhaTuyenDung)
    {
        _context.DsNhaTuyenDung.Update(nhaTuyenDung);
        _context.SaveChanges();
    }
}
