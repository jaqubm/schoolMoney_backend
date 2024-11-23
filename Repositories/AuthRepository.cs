using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class AuthRepository(IConfiguration config) : IAuthRepository
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
    
    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _entityFramework
            .User
            .FindAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<bool> CheckUserExistAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(a => a.Email == email) is not null;
    }
}