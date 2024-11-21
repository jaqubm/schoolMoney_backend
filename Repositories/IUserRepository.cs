using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IUserRepository
{
    public Task<bool> SaveChangesAsync();

    public void DeleteEntity<T>(T entityToDelete);

    public Task<User?> GetUserByIdAsync(string userId);
}