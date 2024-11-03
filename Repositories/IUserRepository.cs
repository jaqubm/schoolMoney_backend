using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IUserRepository
{
    public bool SaveChanges();

    public void DeleteEntity<T>(T entityToDelete);

    public User GetUser(string email);
}