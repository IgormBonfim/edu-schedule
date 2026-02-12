namespace EduSchedule.Application.Auth.Dtos.Responses
{
    public record LoginResponse(UserResponse User, string Token);
    public record UserResponse(string id, string email, string name, string role);
}