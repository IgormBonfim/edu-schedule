using EduSchedule.Application.Auth.Dtos.Requests;
using EduSchedule.Application.Auth.Dtos.Responses;

namespace EduSchedule.Application.Auth.Services.Interfaces
{
    public interface IAuthAppService
    {
        LoginResponse Login(LoginRequest request);
    }
}