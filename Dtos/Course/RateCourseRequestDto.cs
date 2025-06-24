using System.ComponentModel.DataAnnotations;

namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class RateCourseRequestDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string ReviewText { get; set; }
    }
} 