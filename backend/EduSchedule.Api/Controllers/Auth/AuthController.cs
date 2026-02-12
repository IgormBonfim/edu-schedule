using EduSchedule.Application.Auth.Dtos.Requests;
using EduSchedule.Application.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduSchedule.Api.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authAppService;

        public AuthController(IAuthAppService authAppService)
        {
            _authAppService = authAppService;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _authAppService.Login(request);
            return Ok(response);
        }
    }
}
