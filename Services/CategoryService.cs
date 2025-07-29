using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Category;
using Sep490_Eduseen_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sep490_Eduseen_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace Sep490_Eduseen_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly Sep490EduseenContext _context;
        private readonly IMapper _mapper;
        public CategoryService(Sep490EduseenContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesWithCourseCountAsync()
        {
            var categories = await _context.Categories.Include(c => c.Courses).ToListAsync();
            // Map sang DTO nếu cần
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
} 