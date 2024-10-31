using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace schoolMoney_backend.Helpers;

public class AuthHelper(IConfiguration config)
{
    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        var passwordKeySalt = config.GetSection("AppSettings:PasswordKey").Value + 
                              Convert.ToBase64String(passwordSalt);

        var passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordKeySalt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1600000,
            numBytesRequested: 256 / 8);

        return passwordHash;
    }
    
    public string CreateToken(string userId, string email)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email)
        };

        var tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                config.GetSection("AppSettings:TokenKey").Value ??= ""));

        var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}