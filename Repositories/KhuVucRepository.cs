namespace StudentJob.Repositories;

using StudentJob.Data;
using StudentJob.Models;

public class KhuVucRepository : IKhuVucRepository
{
    private readonly AppDbContext _context;

    public KhuVucRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<KhuVuc> GetAll()
    {
        return _context.DsKhuVuc.OrderBy(k => k.sTenKhuVuc).ToList();
    }

    public KhuVuc? GetById(int id)
    {
        return _context.DsKhuVuc.FirstOrDefault(k => k.PK_IdKhuVuc == id);
    }
}
