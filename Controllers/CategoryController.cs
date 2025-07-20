using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICourseService _courseService;
        public CategoryController(ICategoryService categoryService, ICourseService courseService)
        {
            _categoryService = categoryService;
            _courseService = courseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesWithCourseCountAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId:int}/courses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoursesByCategory(int categoryId)
        {
            int? studentId = null;
            var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(studentIdString, out var sid))
            {
                studentId = sid;
            }

            var courses = await _courseService.GetCoursesByCategoryAsync(categoryId, studentId);
            return Ok(courses);
        }
    }
} 