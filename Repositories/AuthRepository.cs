using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Data;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public class AuthRepository(IConfiguration config) : IUserRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public bool SaveChanges()
    {
        return _entityFramework
            .SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        if (entityToAdd is not null)
            _entityFramework
                .Add(entityToAdd);
    }

    public User GetUser(string email)
    {
        var authUser = _entityFramework
            .User
            .FirstOrDefault(a => a.Email == email);

        if (authUser is null) throw new Exception("Failed to Get User");

        return authUser;
    }

    public string GetUserId(string email)
    {
        var userDb = _entityFramework
            .User
            .FirstOrDefault(u => u.Email == email);

        if (userDb is null) throw new Exception("Failed to Get User");
    
        return userDb.UserId;
    }

    public bool CheckUserExist(string email)
    {
        return _entityFramework
            .User
            .FirstOrDefault(a => a.Email == email) is not null;
    }

    public bool CheckUserIdExist(string userId)
    {
        return _entityFramework
            .User
            .FirstOrDefault(u => u.UserId == userId) is not null;
    }
}