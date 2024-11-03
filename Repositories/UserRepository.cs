using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class UserRepository(IConfiguration config) : IUserRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public bool SaveChanges()
    {
        return _entityFramework
            .SaveChanges() > 0;
    }

    public void DeleteEntity<T>(T entityToDelete)
    {
        if (entityToDelete is not null)
            _entityFramework
                .Remove(entityToDelete);
    }

    public User GetUser(string email)
    {
        var authUser = _entityFramework
            .User
            .Include(u => u.Children)
            .FirstOrDefault(a => a.Email == email);

        if (authUser is null) throw new Exception("Failed to Get User");

        return authUser;
    }
}