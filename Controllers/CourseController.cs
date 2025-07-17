using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Sep490_Eduseen_BE.Exceptions;

namespace Sep490_Eduseen_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CourseController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("search/{courseName}")]
        public async Task<IActionResult> SearchCourses(string courseName)
        {
            var courses = await _courseService.SearchCoursesAsync(courseName);
            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("detail/{courseId:int}")]
        public async Task<IActionResult> GetCourseById(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var courses = await _courseService.GetMyCoursesAsync(studentId);
            return Ok(courses);
        }

        [HttpPost("enroll/{courseId:int}")]
        public async Task<IActionResult> EnrollInCourse(int courseId)
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var result = await _enrollmentService.EnrollCourseAsync(studentId, courseId);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(new { message = result.Message });
                }
                return Conflict(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetCourseById), new { courseId = result.Enrollment.CourseId }, result.Enrollment);
        }

        [HttpPost("favorite/{courseId:int}")]
        public async Task<IActionResult> SaveFavoriteCourse(int courseId)
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var result = await _courseService.SaveFavoriteCourseAsync(studentId, courseId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpGet("materials/{courseId:int}")]
        public async Task<IActionResult> GetCourseMaterials(int courseId)
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var result = await _courseService.GetCourseMaterialsAsync(studentId, courseId);

            if (!result.Success)
            {
                return Forbid(result.Message);
            }

            return Ok(result.Data);
        }
                                                                                                                            
        [HttpGet("track/{courseId:int}")]
        public async Task<IActionResult> TrackCourseProgress(int courseId)
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            try
            {
                var progress = await _courseService.GetCourseProgressAsync(studentId, courseId);

                if (progress == null)
                {
                    return Forbid("You are not enrolled in this course to track its progress.");
                }

                return Ok(progress);
            }
            catch (CourseNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("complete-course")]
        public async Task<IActionResult> GetCompletedCourses()
        {
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var courses = await _courseService.GetCompletedCoursesAsync(studentId);
            return Ok(courses);
        }

        [HttpPost("rate/{courseId:int}")]
        public async Task<IActionResult> RateCourse(int courseId, [FromBody] Sep490_Eduseen_BE.Dtos.Course.RateCourseRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(studentIdString, out var studentId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var result = await _courseService.RateCourseAsync(studentId, courseId, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [AllowAnonymous]
        [HttpGet("top-review")]
        public async Task<IActionResult> GetTopReviews()
        {
            var reviews = await _courseService.GetTopReviewsAsync();
            return Ok(reviews);
        }
    }
} 