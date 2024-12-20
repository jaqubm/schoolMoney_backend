using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IUserRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);

    public Task<User?> GetUserByIdAsync(string userId);
    
    public Task<Child?> GetChildByIdAsync(string childId);
    public Task<Account?> GetAccountByAccountNumberAsync(string accountNumber);
    public Task<Class?> GetClassByIdAsync(string classId);
    public Task<List<Class>> GetClassListByTreasurerIdAsync(string treasurerId);
}