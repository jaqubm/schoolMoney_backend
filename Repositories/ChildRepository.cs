using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class ChildRepository(IConfiguration config) : IChildRepository
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

    public async Task<Child?> GetChildByIdAsync(string childId)
    {
        return await _entityFramework
            .Child
            .Include(c => c.Class)
            .ThenInclude(c => c.Fundraises)
            .FirstOrDefaultAsync(c => c.ChildId == childId);
    }
}