namespace StudentJob.Repositories;

using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Models;

public class TaiKhoanRepository : ITaiKhoanRepository
{
    private readonly AppDbContext _context;

    public TaiKhoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public TaiKhoan? GetByEmail(string email)
    {
        return _context.DsTaiKhoan
            .Include(t => t.VaiTro)
            .FirstOrDefault(t => t.sEmail == email);
    }

    public TaiKhoan? GetById(int id)
    {
        return _context.DsTaiKhoan
            .Include(t => t.VaiTro)
            .FirstOrDefault(t => t.PK_IdTaiKhoan == id);
    }

    public List<TaiKhoan> GetAll()
    {
        return _context.DsTaiKhoan
            .Include(t => t.VaiTro)
            .OrderByDescending(t => t.dNgayTaoTaiKhoan)
            .ToList();
    }

    public void Add(TaiKhoan taiKhoan)
    {
        _context.DsTaiKhoan.Add(taiKhoan);
        _context.SaveChanges();
    }

    public void Update(TaiKhoan taiKhoan)
    {
        _context.DsTaiKhoan.Update(taiKhoan);
        _context.SaveChanges();
    }

    public void UpdateTrangThai(int id, bool active)
    {
        var taiKhoan = _context.DsTaiKhoan.FirstOrDefault(t => t.PK_IdTaiKhoan == id);
        if (taiKhoan != null)
        {
            taiKhoan.bTrangThaiHoatDong = active;
            _context.SaveChanges();
        }
    }
}
