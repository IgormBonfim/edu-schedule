using EduSchedule.Application.Students.Dtos.Requests;
using EduSchedule.Application.Students.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduSchedule.Api.Controllers.Students
{
    [Route("api/students")]
    [Authorize]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsAppService _studentsAppService;

        public StudentsController(IStudentsAppService studentsAppService)
        {
            _studentsAppService = studentsAppService;
        }
        [HttpGet]
        public async Task<IActionResult> GetStudentsAsync([FromQuery] ListStudentsRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _studentsAppService.GetStudentsAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
