using Sep490_Eduseen_BE.Dtos.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesWithCourseCountAsync();
    }
} 