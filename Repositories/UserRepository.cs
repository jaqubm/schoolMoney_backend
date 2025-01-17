using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class UserRepository(IConfiguration config) : IUserRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _entityFramework
            .SaveChangesAsync() > 0;
    }
    
    public async Task AddEntityAsync<T>(T entity)
    {
        if (entity is not null)
            await _entityFramework
                .AddAsync(entity);
    }

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public void DeleteEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Remove(entity);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _entityFramework
            .User
            .Include(u => u.Account)
            .Include(u => u.Children)
            .Include(u => u.ClassesAsTreasurer)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<User?> GetUserWithAccountByIdAsync(string userId)
    {
        return await _entityFramework
            .User
            .Include(u => u.Account)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }
    
    public async Task<bool> UserWithGivenEmailExistsAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(a => a.Email == email) is not null;
    }
}