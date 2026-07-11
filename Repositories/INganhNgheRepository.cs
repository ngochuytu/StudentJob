namespace StudentJob.Repositories;

using StudentJob.Models;

public interface INganhNgheRepository
{
    List<NganhNghe> GetAll();
    NganhNghe? GetById(int id);
}
