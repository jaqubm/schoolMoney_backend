using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IAuthRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
}