namespace StudentJob.Repositories;

using StudentJob.Models;

public interface IKhuVucRepository
{
    List<KhuVuc> GetAll();
    KhuVuc? GetById(int id);
}
