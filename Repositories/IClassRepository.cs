using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IClassRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Task<Class?> GetClassByIdAsync(string classId);
    
    public Task<List<Class>> GetClassListByTreasurerIdAsync(string treasurerId);
    public Task<List<Class>> GetClassListByNameThatStartsWithAsync(string className);
    
    public Task<bool> ClassWithGivenSchoolAndClassNameExistsAsync(string schoolName, string className);
}