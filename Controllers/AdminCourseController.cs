using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;
using Sep490_Eduseen_BE.Dtos.Course;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Controllers
{
    [Route("api/admin/course")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public AdminCourseController(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoursesForAdmin()
        {
            var courses = await _courseService.GetAllCoursesForAdminAsync();
            return Ok(courses);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetCourseStatistics()
        {
            var statistics = await _courseService.GetCourseStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> GetCourseById(int courseId)
        {
            var course = await _courseService.GetCourseByIdForAdminAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPut("{courseId:int}/status")]
        public async Task<IActionResult> UpdateCourseStatus(int courseId, [FromBody] UpdateCourseStatusDto request)
        {
            var result = await _courseService.UpdateCourseStatusAsync(courseId, request.IsActive);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{courseId:int}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var result = await _courseService.DeleteCourseByAdminAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCourses()
        {
            var courses = await _courseService.GetPendingCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("by-teacher/{teacherId:int}")]
        public async Task<IActionResult> GetCoursesByTeacher(int teacherId)
        {
            var courses = await _courseService.GetCoursesByTeacherAsync(teacherId);
            return Ok(courses);
        }
    }
} 