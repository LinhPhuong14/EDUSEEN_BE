using Sep490_Eduseen_BE.Dtos.Course;

namespace Sep490_Eduseen_BE.Dtos
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public int TeacherId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Cover { get; set; } // Thêm trường cover
        public List<SectionDTO> Sections { get; set; } = new();
    }

    public class SectionDTO
    {
        public int SectionId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<LectureDTO> Lectures { get; set; } = new();
    }

    public class LectureDTO
    {
        public int LectureId { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
        public int? AssignmentId { get; set; } // ID của assignment liên quan đến lecture này
    }

    public class CreateCourseDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public string? Cover { get; set; } // Thêm trường cover
        public List<CreateSectionDTO> Sections { get; set; } = new();
    }

    public class CreateSectionDTO
    {
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<CreateLectureDTO> Lectures { get; set; } = new();
    }

    public class CreateLectureDTO
    {
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
        public int? AssignmentId { get; set; } // ID của assignment liên quan đến lecture này
    }

    public class UpdateCourseDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public string? Cover { get; set; } // Thêm trường cover
        public List<UpdateSectionDTO> Sections { get; set; } = new();
    }

    public class UpdateSectionDTO
    {
        public int? SectionId { get; set; } // null nếu là section mới
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<UpdateLectureDTO> Lectures { get; set; } = new();
    }

    public class UpdateLectureDTO
    {
        public int? LectureId { get; set; } // null nếu là lecture mới
        public string Title { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContentUrl { get; set; }
        public int? Duration { get; set; }
        public int Order { get; set; }
        public int? AssignmentId { get; set; } // ID của assignment liên quan đến lecture này
    }

    public class CourseAnalysisDTO
    {
        public int CourseId { get; set; }
        public int TotalEnrollments { get; set; }
        public double CompletionRate { get; set; }
        public double AverageRating { get; set; }
        public double AvgCompletedLectures { get; set; }
        public List<AssignmentAnalysisDTO> Assignments { get; set; } = new();
    }

    public class AssignmentAnalysisDTO
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public int TotalAssigned { get; set; } // Tổng số học viên được giao
        public int TotalSubmitted { get; set; } // Số học viên đã nộp
        public double CompletionRate { get; set; } // Tỉ lệ nộp bài
        public int LateSubmissionCount { get; set; } // Số bài nộp muộn
        public double LateSubmissionRate { get; set; } // Tỉ lệ nộp muộn
        public double AverageGrade { get; set; } // Điểm trung bình
        public Dictionary<string, int> GradeDistribution { get; set; } = new(); // Phân bổ điểm
        public int GradedCount { get; set; } // Số bài đã chấm điểm
    }

}
