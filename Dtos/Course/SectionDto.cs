using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class SectionDto
    {
        public int SectionId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public List<LectureDto> Lectures { get; set; } = new List<LectureDto>();
    }
} 