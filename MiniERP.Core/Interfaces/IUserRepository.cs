using MiniERP.Core.Entities;

namespace MiniERP.Core.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User?> GetById(int id);
    Task<List<User>> GetAll();
    Task Add(User user);
    Task Update(User user);
    Task Delete(User user);
    Task<int> CountAdmins();
}