using schoolMoney_backend.Models;

namespace schoolMoney_backend.Repositories;

public interface IAuthRepository
{
    public bool SaveChanges();

    public void AddEntity<T>(T entityToAdd);

    public User GetUser(string email);
    public string GetUserId(string email);
    
    public bool CheckUserExist(string email);
    public bool CheckUserIdExist(string userId);
}