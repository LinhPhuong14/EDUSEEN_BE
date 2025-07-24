using Microsoft.AspNetCore.Http;

namespace Sep490_Eduseen_BE.Dtos.Category
{
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; }
        public IFormFile? Cover { get; set; }
        public IFormFile? HoverCover { get; set; }
    }
} 