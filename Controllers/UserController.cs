using System.Security.Cryptography;
using AutoMapper;
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
public class UserController(IConfiguration config, IUserRepository userRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<User, UserDto>();
    }));

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            return BadRequest("Passwords do not match!");
        
        if (userRepository.CheckUserExist(userForRegistration.Email))
            return BadRequest("User with this email already exists!");
        
        var passwordSalt = new byte[128 / 8];
        
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
            randomNumberGenerator.GetNonZeroBytes(passwordSalt);
        
        var passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

        var userAccount = new Account();
        
        userRepository.AddEntity(new User(
            userForRegistration.Email,
            passwordHash,
            passwordSalt)
            {
                Account = userAccount
            }
        );
        
        return userRepository.SaveChanges() ? Ok() : Problem("Failed to Register User");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        var authUser = userRepository.GetUser(userForLogin.Email);

        var passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, authUser.PasswordSalt);

        if (passwordHash.Where((t, i) => t != authUser.PasswordHash[i]).Any())
            return StatusCode(401, "Incorrect password!");

        var userId = userRepository.GetUserId(userForLogin.Email);
        
        return Ok(new Dictionary<string, string> { { "Token", _authHelper.CreateToken(userId, authUser.Email) } });
    }
    
    [HttpGet("Get/{email}")]
    public ActionResult<UserDto> GetUser([FromRoute] string email)
    {
        var user = userRepository.GetUser(email);
        
        return Ok(_mapper.Map<UserDto>(user));
    }
}