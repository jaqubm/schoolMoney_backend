using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class FundraiseRepository(IConfiguration config) : IFundraiseRepository
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
    
    public async Task<Account?> GetAccountByAccountNumberAsync(string accountNumber)
    {
        return await _entityFramework
            .Account
            .Include(a => a.SourceTransactions)
            .Include(a => a.DestinationTransactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Fundraise?> GetFundraiseByIdAsync(string fundraiseId)
    {
        return await _entityFramework
            .Fundraise
            .Include(f => f.Class)
            .Include(f => f.Account)
            .ThenInclude(a => a.DestinationTransactions)
            .FirstOrDefaultAsync(f => f.FundraiseId == fundraiseId);
    }
}