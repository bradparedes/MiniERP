using System.Threading.Tasks;
using MiniERP.Core.Entities;

namespace MiniERP.Core.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User?> GetById(int id);
    Task<List<User>> GetAll();
    Task Add(User user);
    Task Update(User user);
    void Delete(User user);
    Task<int> CountAdmins();
}