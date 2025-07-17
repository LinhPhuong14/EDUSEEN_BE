namespace Sep490_Eduseen_BE.Dtos
{
    public class AssignmentDetailDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string SubmissionStatus { get; set; } = "Chưa nộp";
        public DateTime? SubmittedAt { get; set; }
        public decimal? Grade { get; set; }

        public int LectureId { get; set; }            
        public string? LectureTitle { get; set; }     
    }

}
