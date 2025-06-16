using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Dtos;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/teacher/course")]
    [Authorize]
    public class TeacherCourseController : ControllerBase
    {

        private readonly ICourseService _service;
        public TeacherCourseController(ICourseService service) => _service = service;

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var teacherId = GetTeacherId();
            var course = await _service.GetCourseAsync(courseId, teacherId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var teacherId = GetTeacherId();
            var course = await _service.CreateCourseAsync(dto, teacherId);
            return CreatedAtAction(nameof(GetCourse), new { courseId = course.CourseId }, course);
        }

        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] UpdateCourseDto dto)
        {
            var teacherId = GetTeacherId();
            var updated = await _service.UpdateCourseAsync(courseId, dto, teacherId);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var teacherId = GetTeacherId();
            var deleted = await _service.DeleteCourseAsync(courseId, teacherId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private int GetTeacherId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("No NameIdentifier claim found in token.");
            return int.Parse(claim.Value);
        }

    }
}
