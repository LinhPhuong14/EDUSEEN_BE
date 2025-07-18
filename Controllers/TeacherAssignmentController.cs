using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Repositories;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/teacher/assignment")]
    [Authorize]
    public class TeacherAssignmentController : ControllerBase
    {
        private readonly ICourseTeacherService _service;
        public TeacherAssignmentController(ICourseTeacherService service)
        {
            _service = service;
        }

        private int GetTeacherId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("No NameIdentifier claim found in token.");
            return int.Parse(claim.Value);
        }

        // GET api/teacher/assignment/{assignmentId}/submissions
        [HttpGet("{assignmentId}/submissions")]
        public async Task<IActionResult> GetSubmissions(int assignmentId)
        {
            var teacherId = GetTeacherId();
            var result = await _service.GetAssignmentSubmissionsAsync(assignmentId, teacherId);
            return Ok(result);
        }
    }
} 