namespace EduSchedule.Domain.Auth.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(string id, string name, string email, string role);
}
