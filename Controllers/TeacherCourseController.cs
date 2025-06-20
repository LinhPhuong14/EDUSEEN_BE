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
        private readonly IReviewService _rvservice;
        public TeacherCourseController(IReviewService rvservice, ICourseService service)
        {
            _service = service;
            _rvservice = rvservice;
        }
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

        [HttpGet("{courseId}/analysis")]
        public async Task<IActionResult> GetCourseAnalysis(int courseId)
        {
            var teacherId = GetTeacherId();
            var result = await _service.GetCourseAnalysisAsync(courseId, teacherId);
            return Ok(result);
        }

        [HttpPost("review/{reviewId}/response")]
        public async Task<IActionResult> RespondToReview(int reviewId, [FromBody] RespondReviewDTO dto)
        {
            var teacherId = GetTeacherId();
            var response = await _rvservice.RespondToReviewAsync(reviewId, teacherId, dto.ResponseText);
            return Ok(response);
        }
        [HttpGet("assignment/{assignmentId}/analysis")]
        public async Task<IActionResult> GetHomeworkAnalysis(int assignmentId)
        {
            var teacherId = GetTeacherId(); 
            var analysis = await _service.GetHomeworkAnalysisAsync(assignmentId, teacherId);
            return Ok(analysis);
        }


    }
}
