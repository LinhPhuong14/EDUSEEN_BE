using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sep490_Eduseen_BE.Dtos
{
    public class UploadRequestDTO
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public string? SubmissionContent { get; set; }

        [Required]
        public List<IFormFile> Files { get; set; } = new();
    }
}
