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
    public async Task<ActionResult<string>> Register(UserRegisterDto userRegisterDto)
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
        
        await authRepository.AddEntityAsync(new User
            {
                Email = userRegisterDto.Email,
                Name = userRegisterDto.Name,
                Surname = userRegisterDto.Surname,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Account = userAccount
            }
        );
        
        return await authRepository.SaveChangesAsync() ? Ok() : Problem("Failed to Register User!");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(UserLoginDto userLoginDto)
    {
        var userDb = await authRepository.GetUserByEmailAsync(userLoginDto.Email);
        if (userDb is null) return NotFound("User not found!");

        var passwordHash = _authHelper.GetPasswordHash(userLoginDto.Password, userDb.PasswordSalt);

        return passwordHash.Where((t, i) => t != userDb.PasswordHash[i]).Any() 
            ? BadRequest("Wrong Password!")
            : Ok(new Dictionary<string, string> { { "Token", _authHelper.CreateToken(userDb.UserId, userDb.Email) } });
    }

    [HttpPut("UpdatePassword")]
    public async Task<ActionResult<string>> UpdatePassword(UserPasswordUpdateDto userPasswordUpdateDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await authRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        
        if (userPasswordUpdateDto.NewPassword != userPasswordUpdateDto.NewPasswordConfirm)
            return BadRequest("New password does not match password confirm!");
        
        var oldPasswordHash = _authHelper.GetPasswordHash(userPasswordUpdateDto.OldPassword, userDb.PasswordSalt);
        
        if (oldPasswordHash.Where((t, i) => t != userDb.PasswordHash[i]).Any()) 
            return BadRequest("Wrong password!");
        
        var newPasswordSalt = new byte[128 / 8];
        
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
            randomNumberGenerator.GetNonZeroBytes(newPasswordSalt);
        
        var newPasswordHash = _authHelper.GetPasswordHash(userPasswordUpdateDto.NewPassword, newPasswordSalt);
        
        userDb.PasswordHash = newPasswordHash;
        userDb.PasswordSalt = newPasswordSalt;
        
        authRepository.UpdateEntity(userDb);
        
        return await authRepository.SaveChangesAsync() ? Ok() : Problem("Failed to Update User password!");
    }
}