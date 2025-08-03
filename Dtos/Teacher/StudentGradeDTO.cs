namespace Sep490_Eduseen_BE.Dtos.Teacher
{
    public class StudentGradeDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public List<AssignmentGradeDTO> AssignmentGrades { get; set; } = new List<AssignmentGradeDTO>();
        public double AverageGrade { get; set; }
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
    }

               public class AssignmentGradeDTO
           {
               public int AssignmentId { get; set; }
               public string AssignmentTitle { get; set; } = string.Empty;
               public double? Grade { get; set; }
               public DateTime? SubmittedAt { get; set; }
               public string Status { get; set; } = string.Empty; // "Submitted", "Graded", "Not Submitted"
           }
} 