using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Category;
using Sep490_Eduseen_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesWithCourseCountAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithCourseCountAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
} 