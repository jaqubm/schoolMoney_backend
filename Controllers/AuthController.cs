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
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        if (userRegisterDto.Password != userRegisterDto.PasswordConfirm)
            return BadRequest("Passwords do not match!");
        
        if (await authRepository.CheckUserExistAsync(userRegisterDto.Email))
            return BadRequest("User with this email already exists!");
        
        var passwordSalt = new byte[128 / 8];
        
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
            randomNumberGenerator.GetNonZeroBytes(passwordSalt);
        
        var passwordHash = _authHelper.GetPasswordHash(userRegisterDto.Password, passwordSalt);

        var userAccount = new Account();
        
        await authRepository.AddEntityAsync(new User(
            userRegisterDto.Email,
            userRegisterDto.Name,
            userRegisterDto.Surname,
            passwordHash,
            passwordSalt)
            {
                Account = userAccount
            }
        );
        
        return await authRepository.SaveChangesAsync() ? Ok() : Problem("Failed to Register User");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        var userDb = await authRepository.GetUserByEmailAsync(userLoginDto.Email);
        if (userDb is null) return NotFound("User not found!");

        var passwordHash = _authHelper.GetPasswordHash(userLoginDto.Password, userDb.PasswordSalt);

        return passwordHash.Where((t, i) => t != userDb.PasswordHash[i]).Any() 
            ? StatusCode(401, "Incorrect password!") 
            : Ok(new Dictionary<string, string> { { "Token", _authHelper.CreateToken(userDb.UserId, userDb.Email) } });
    }
}