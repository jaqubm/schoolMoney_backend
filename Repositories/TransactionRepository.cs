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

    public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
    {
        return await _entityFramework
            .Transaction
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }
}