namespace Sep490_Eduseen_BE.Dtos
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public int TeacherId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
    }

    public class SectionDto
    {
        public int SectionId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<LectureDto> Lectures { get; set; } = new();
    }

    public class LectureDto
    {
        public int LectureId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
    }

    public class CreateCourseDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public List<CreateSectionDto> Sections { get; set; } = new();
    }

    public class CreateSectionDto
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<CreateLectureDto> Lectures { get; set; } = new();
    }

    public class CreateLectureDto
    {
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
    }

    public class UpdateCourseDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public List<UpdateSectionDto> Sections { get; set; } = new();
    }

    public class UpdateSectionDto
    {
        public int? SectionId { get; set; } // null nếu là section mới
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<UpdateLectureDto> Lectures { get; set; } = new();
    }

    public class UpdateLectureDto
    {
        public int? LectureId { get; set; } // null nếu là lecture mới
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
    }

    public class CourseAnalysisDto
    {
        public int CourseId { get; set; }
        public int TotalEnrollments { get; set; }
        public double CompletionRate { get; set; }
        public double AverageRating { get; set; }
        public double AvgCompletedLectures { get; set; }
    }

}
