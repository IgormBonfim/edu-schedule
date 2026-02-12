using System.Text;
using EduSchedule.Infrastructure.Auth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EduSchedule.Api.Extensions;

public static class ConfigureAuth
{
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtOptions");

        var securityKeyParameter = configuration.GetSection("JwtOptions:SecurityKey").Value;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyParameter));

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSettings[nameof(JwtOptions.Issuer)]!;
            options.Audience = jwtSettings[nameof(JwtOptions.Audience)]!;
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            options.Expiration = int.Parse(jwtSettings[nameof(JwtOptions.Expiration)] ?? "0");
        });


        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration.GetSection("JwtOptions:Issuer").Value,

            ValidateAudience = true,
            ValidAudience = configuration.GetSection("JwtOptions:Audience").Value,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });

        services.AddAuthorization();
    }
}
