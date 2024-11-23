using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IUserRepository
{
    public Task<bool> SaveChangesAsync();

    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);

    public Task<User?> GetUserByIdAsync(string userId);
}