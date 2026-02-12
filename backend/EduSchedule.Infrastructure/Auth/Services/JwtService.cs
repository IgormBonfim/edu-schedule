using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EduSchedule.Domain.Auth.Services.Interfaces;
using EduSchedule.Infrastructure.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EduSchedule.Infrastructure.Auth.Services;

public class JwtService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(string id, string name, string email, string role)
    {
        var handler = new JwtSecurityTokenHandler();

        DateTime dataExpiracao = DateTime.UtcNow.AddHours(_jwtOptions.Expiration);

        IList<Claim> tokenClaims = new List<Claim>
        {
            new("Email", email),
            new("Username", name),
            new("UserId", id),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(tokenClaims),
            NotBefore = DateTime.UtcNow,
            Expires = dataExpiracao,
            SigningCredentials = _jwtOptions.SigningCredentials,
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}
