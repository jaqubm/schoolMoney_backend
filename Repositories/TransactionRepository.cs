using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class TransactionRepository(IConfiguration config) : ITransactionRepository
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
            .Include(u => u.Account.SourceTransactions)
            .Include(u => u.Account.DestinationTransactions)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<Account?> GetAccountByAccountNumberAsync(string accountNumber)
    {
        return await _entityFramework
            .Account
            .Include(a => a.SourceTransactions)
            .Include(a => a.DestinationTransactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
    {
        return await _entityFramework
            .Transaction
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }
}