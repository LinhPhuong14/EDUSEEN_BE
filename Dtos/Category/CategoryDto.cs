namespace Sep490_Eduseen_BE.Dtos.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseCount { get; set; }
        public string? Cover { get; set; }
        public string? HoverCover { get; set; }
    }
} 