namespace StudentJob.Repositories;

using StudentJob.Data;
using StudentJob.Models;

public class NganhNgheRepository : INganhNgheRepository
{
    private readonly AppDbContext _context;

    public NganhNgheRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<NganhNghe> GetAll()
    {
        return _context.DsNganhNghe.OrderBy(n => n.sTenLinhVuc).ToList();
    }

    public NganhNghe? GetById(int id)
    {
        return _context.DsNganhNghe.FirstOrDefault(n => n.PK_IdNganhNghe == id);
    }
}
