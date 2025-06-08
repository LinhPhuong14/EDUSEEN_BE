using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sep490_Eduseen_BE.Dtos
{

    public class SubmissionUploadDto
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public List<IFormFile> Files { get; set; }

        public string SubmissionContent { get; set; }
    }
}