using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IAuthRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);

    public Task<User?> GetUserByIdAsync(string userId);
    public Task<User?> GetUserByEmailAsync(string email);
    
    public Task<bool> CheckUserExistAsync(string email);
}