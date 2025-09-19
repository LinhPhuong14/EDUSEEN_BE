using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Dtos;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/teacher/course")]
    [Authorize]
    public class TeacherCourseController : ControllerBase
    {

        private readonly ICourseTeacherService _service;
        private readonly IReviewService _rvservice;
        private readonly Sep490EduseenContext _context;
        public TeacherCourseController(IReviewService rvservice, ICourseTeacherService service, Sep490EduseenContext context)
        {
            _service = service;
            _rvservice = rvservice;
            _context = context;
        }
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var teacherId = GetTeacherId();
            var course = await _service.GetCourseAsync(courseId, teacherId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // GET api/teacher/course
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var teacherId = GetTeacherId();
            var courses = await _service.GetCoursesAsync(teacherId);
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO dto)
        {
            var teacherId = GetTeacherId();
            var course = await _service.CreateCourseAsync(dto, teacherId);
            return CreatedAtAction(nameof(GetCourse), new { courseId = course.CourseId }, course);
        }

        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] UpdateCourseDTO dto)
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

        // lấy danh sách review + thông tin khoá học
        [HttpGet("{courseId}/reviews")]
        public async Task<IActionResult> GetCourseReviews(int courseId)
        {
            var teacherId = GetTeacherId();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
            if (course == null) return NotFound("Khoá học không tồn tại hoặc bạn không có quyền.");

            var reviewsData = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.ReviewResponses)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviews = reviewsData.Select(r => new {
                id = r.ReviewId,
                name = ($"{r.Student.FirstName} {r.Student.LastName}").Trim(),
                rating = r.Rating,
                desc = r.Comment,
                date = r.CreatedAt?.ToString("dd/MM/yyyy"),
                teacherReply = r.ReviewResponses.Select(x => x.ResponseText).FirstOrDefault(),
                responseId = r.ReviewResponses.Select(x => x.ResponseId).FirstOrDefault()
            }).ToList();

            return Ok(new
            {
                courseId = course.CourseId,
                title = course.Title,
                cover = course.Cover,
                reviews
            });
        }

        // teacher reply review
        [HttpPost("review/{reviewId}/reply")]
        public async Task<IActionResult> ReplyReview(int reviewId, [FromBody] RespondReviewDTO dto)
        {
            var teacherId = GetTeacherId();
            var response = await _rvservice.RespondToReviewAsync(reviewId, teacherId, dto.ResponseText);
            return Ok(response);
        }

        // teacher update review reply
        [HttpPut("review/response/{responseId}")]
        public async Task<IActionResult> UpdateReviewReply(int responseId, [FromBody] RespondReviewDTO dto)
        {
            var teacherId = GetTeacherId();
            try
            {
                var response = await _rvservice.UpdateReviewResponseAsync(responseId, teacherId, dto.ResponseText);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // teacher delete review reply
        [HttpDelete("review/response/{responseId}")]
        public async Task<IActionResult> DeleteReviewReply(int responseId)
        {
            var teacherId = GetTeacherId();
            var deleted = await _rvservice.DeleteReviewResponseAsync(responseId, teacherId);
            if (!deleted) return NotFound("Response not found or unauthorized");
            return NoContent();
        }

        [HttpGet("{courseId}/assignments")]
        public async Task<IActionResult> GetAssignments(int courseId)
        {
            var teacherId = GetTeacherId();
            var assignments = await _service.GetAssignmentsAsync(courseId, teacherId);
            return Ok(assignments);
        }
        [HttpGet("assignment/{assignmentId}/analysis")]
        public async Task<IActionResult> GetHomeworkAnalysis(int assignmentId)
        {
            var teacherid = GetTeacherId();
            var analysis = await _service.GetHomeworkAnalysisAsync(assignmentId, teacherid);
            return Ok(analysis);
        }

        [HttpGet("{courseId}/students/grades")]
        public async Task<IActionResult> GetStudentGrades(int courseId)
        {
            var teacherId = GetTeacherId();
            var studentGrades = await _service.GetStudentGradesAsync(courseId, teacherId);
            return Ok(studentGrades);
        }


    }
}
