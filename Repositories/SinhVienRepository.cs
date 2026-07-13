namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class SinhVienRepository : ISinhVienRepository
{
    private readonly AppDbContext _context;

    public SinhVienRepository(AppDbContext context)
    {
        _context = context;
    }

    public SinhVien? GetByTaiKhoanId(int taiKhoanId)
    {
        return _context
            .DsSinhVien.Include(s => s.TaiKhoan)
            .FirstOrDefault(s => s.FK_IdTaiKhoan == taiKhoanId);
    }

    public SinhVien? GetById(int id)
    {
        return _context
            .DsSinhVien.Include(s => s.TaiKhoan)
            .FirstOrDefault(s => s.PK_IdSinhVien == id);
    }

    public void Add(SinhVien sinhVien)
    {
        _context.DsSinhVien.Add(sinhVien);
        _context.SaveChanges();
    }

    public void Update(SinhVien sinhVien)
    {
        _context.DsSinhVien.Update(sinhVien);
        _context.SaveChanges();
    }
}
