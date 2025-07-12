namespace Sep490_Eduseen_BE.Dtos
{
    public class ReviewResponseDTO
    {
        public int ResponseId { get; set; }
        public int ReviewId { get; set; }
        public int TeacherId { get; set; }
        public string ResponseText { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
    public class RespondReviewDTO
    {
        public string ResponseText { get; set; } = null!;
    }

}
