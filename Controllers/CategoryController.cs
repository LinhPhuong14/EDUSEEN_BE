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

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        public CategoryController(ICategoryService categoryService, IWebHostEnvironment env)
        {
            _categoryService = categoryService;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesWithCourseCountAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto dto)
        {
            var category = new Category { CategoryName = dto.CategoryName };
            if (dto.Cover != null && !string.IsNullOrEmpty(dto.Cover.FileName))
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Cover.FileName);
                var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads/category", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Cover.CopyToAsync(stream);
                }
                category.Cover = "/uploads/category/" + fileName;
            }
            if (dto.HoverCover != null && !string.IsNullOrEmpty(dto.HoverCover.FileName))
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.HoverCover.FileName);
                var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads/category", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.HoverCover.CopyToAsync(stream);
                }
                category.HoverCover = "/uploads/category/" + fileName;
            }
            await _categoryService.AddCategoryAsync(category);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryCreateDto dto)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            category.CategoryName = dto.CategoryName;
            if (dto.Cover != null && !string.IsNullOrEmpty(dto.Cover.FileName))
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Cover.FileName);
                var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads/category", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Cover.CopyToAsync(stream);
                }
                category.Cover = "/uploads/category/" + fileName;
            }
            if (dto.HoverCover != null && !string.IsNullOrEmpty(dto.HoverCover.FileName))
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.HoverCover.FileName);
                var filePath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads/category", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.HoverCover.CopyToAsync(stream);
                }
                category.HoverCover = "/uploads/category/" + fileName;
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
    }
} 