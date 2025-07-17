using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories.impl
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Sep490EduseenContext _context;
        public CategoryRepository(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesWithCourseCountAsync()
        {
            return await _context.Categories
                .Include(c => c.Courses)
                .ToListAsync();
        }
    }
} 