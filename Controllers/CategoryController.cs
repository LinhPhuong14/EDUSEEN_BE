using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Sep490_Eduseen_BE.Models;
using Microsoft.AspNetCore.Http;
using Sep490_Eduseen_BE.Dtos.Category;
using System.Security.Claims;
using System.Text.Json;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly ICourseService _courseService;

        // Hợp nhất constructor, inject đủ các service cần thiết
        public CategoryController(ICategoryService categoryService, IWebHostEnvironment env, ICourseService courseService)
        {
            _categoryService = categoryService;
            _env = env;
            _courseService = courseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesWithCourseCountAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateWithUrlDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                return BadRequest(new { message = "Tên danh mục không được để trống" });
            }

            var category = new Category { CategoryName = dto.CategoryName.Trim() };
            
            // Xử lý URL từ S3
            if (!string.IsNullOrEmpty(dto.Cover))
            {
                category.Cover = dto.Cover;
            }
            if (!string.IsNullOrEmpty(dto.HoverCover))
            {
                category.HoverCover = dto.HoverCover;
            }
            
            await _categoryService.AddCategoryAsync(category);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryCreateWithUrlDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                return BadRequest(new { message = "Tên danh mục không được để trống" });
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            category.CategoryName = dto.CategoryName.Trim();
            
            // Xử lý URL từ S3
            if (!string.IsNullOrEmpty(dto.Cover))
            {
                category.Cover = dto.Cover;
            }
            if (!string.IsNullOrEmpty(dto.HoverCover))
            {
                category.HoverCover = dto.HoverCover;
            }
            
            await _categoryService.UpdateCategoryAsync(category);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            await _categoryService.DeleteCategoryAsync(category);
            return Ok();
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