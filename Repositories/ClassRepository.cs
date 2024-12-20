using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class ClassRepository(IConfiguration config) : IClassRepository
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
            .Include(u => u.Children)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<Class?> GetClassByIdAsync(string classId)
    {
        return await _entityFramework
            .Class
            .Include(c => c.Treasurer)
            .Include(c => c.Children)
            .Include(c => c.Fundraises)
            .FirstOrDefaultAsync(c => c.ClassId == classId);
    }

    public async Task<List<Class>> SearchClassesByNameAsync(string className)
    {
        var queryable = _entityFramework.Class.AsQueryable();
        
        if (!string.IsNullOrEmpty(className))
            queryable = queryable.Where(c => c.Name.Contains(className));
        
        return await queryable.Take(10).ToListAsync();
    }
}