namespace StudentJob.Repositories;

using StudentJob.Data;
using StudentJob.Models;

public class VaiTroRepository : IVaiTroRepository
{
    private readonly AppDbContext _context;

    public VaiTroRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<VaiTro> GetAll()
    {
        return _context.DsVaiTro.ToList();
    }

    public VaiTro? GetById(int id)
    {
        return _context.DsVaiTro.FirstOrDefault(v => v.PK_IdVaiTro == id);
    }
}
