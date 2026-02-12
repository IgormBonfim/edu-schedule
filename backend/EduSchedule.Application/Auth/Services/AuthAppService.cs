using EduSchedule.Application.Auth.Dtos.Requests;
using EduSchedule.Application.Auth.Dtos.Responses;
using EduSchedule.Application.Auth.Services.Interfaces;
using EduSchedule.Domain.Auth.Services.Interfaces;

namespace EduSchedule.Application.Auth.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly ITokenService _tokenService;

        public AuthAppService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        public LoginResponse Login(LoginRequest request)
        {
            string id = Guid.NewGuid().ToString();
            string name = request.Email.Split('@')[0];
            const string role = "Administrador";
            string token = _tokenService.GenerateToken(id, name, request.Email, role);

            return new LoginResponse(
                User: new UserResponse(id, request.Email, name, role),
                Token: token
            );
        }
    }
}