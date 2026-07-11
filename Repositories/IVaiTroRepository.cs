namespace StudentJob.Repositories;

using StudentJob.Models;

public interface IVaiTroRepository
{
    List<VaiTro> GetAll();
    VaiTro? GetById(int id);
}
