using Sep490_Eduseen_BE.Dtos.Category;
using Sep490_Eduseen_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesWithCourseCountAsync();
        Task AddCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
} 