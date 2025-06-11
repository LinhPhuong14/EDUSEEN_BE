using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly Sep490EduseenContext _context;
    private readonly IWebHostEnvironment _env;

    public SubmissionController(Sep490EduseenContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadSubmission([FromForm] UploadRequestDTO request)
    {
        var previousAttempts = await _context.Submissions
            .Where(s => s.AssignmentId == request.AssignmentId && s.StudentId == request.StudentId)
            .ToListAsync();

        int attemptNumber = previousAttempts.Count + 1;

        var submission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            AttemptNumber = attemptNumber,
            SubmittedAt = DateTime.UtcNow,
            SubmissionContent = request.SubmissionContent,
            Grade = null,
            Feedback = null,
            SubmissionFiles = new List<SubmissionFile>()
        };

        string uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        foreach (var file in request.Files)
        {
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadPath, uniqueFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            submission.SubmissionFiles.Add(new SubmissionFile
            {
                FileName = file.FileName,
                FileUrl = $"/uploads/{uniqueFileName}"
            });
        }

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Bài tập đã được nộp thành công",
            submissionId = submission.SubmissionId,
            attemptNumber = submission.AttemptNumber
        });
    }

}
