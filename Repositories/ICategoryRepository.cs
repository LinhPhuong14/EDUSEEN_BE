using Sep490_Eduseen_BE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesWithCourseCountAsync();
    }
} 