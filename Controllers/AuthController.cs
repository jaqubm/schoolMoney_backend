using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using schoolMoney_backend.Dtos;
using schoolMoney_backend.Helpers;
using schoolMoney_backend.Models;
using schoolMoney_backend.Repositories;

namespace schoolMoney_backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration config, IAuthRepository authRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            return BadRequest("Passwords do not match!");
        
        if (authRepository.CheckUserExist(userForRegistration.Email))
            return BadRequest("User with this email already exists!");
        
        var passwordSalt = new byte[128 / 8];
        
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
            randomNumberGenerator.GetNonZeroBytes(passwordSalt);
        
        var passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

        var userAccount = new Account();
        
        authRepository.AddEntity(new User(
            userForRegistration.Email,
            passwordHash,
            passwordSalt)
            {
                Account = userAccount
            }
        );
        
        return authRepository.SaveChanges() ? Ok() : Problem("Failed to Register User");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        var authUser = authRepository.GetUser(userForLogin.Email);

        var passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, authUser.PasswordSalt);

        if (passwordHash.Where((t, i) => t != authUser.PasswordHash[i]).Any())
            return StatusCode(401, "Incorrect password!");

        var userId = authRepository.GetUserId(userForLogin.Email);
        
        return Ok(new Dictionary<string, string> { { "Token", _authHelper.CreateToken(userId, authUser.Email) } });
    }
}