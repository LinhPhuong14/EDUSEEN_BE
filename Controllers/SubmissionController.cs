using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Sep490_Eduseen_BE.Controllers;

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
    [SwaggerOperation(Summary = "Upload a submission with files")]
    public async Task<IActionResult> UploadSubmission([FromForm] SubmissionUploadDto request)
    {
        var previousAttempts = await _context.Submissions
            .Where(s => s.AssignmentId == request.AssignmentId && s.StudentId == request.StudentId)
            .ToListAsync();

        var attemptNumber = previousAttempts.Count + 1;

        var submission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            AttemptNumber = attemptNumber,
            SubmittedAt = DateTime.UtcNow,
            SubmissionContent = request.SubmissionContent,
            SubmissionFiles = new List<SubmissionFile>()
        };

        var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        foreach (var file in request.Files)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            submission.SubmissionFiles.Add(new SubmissionFile
            {
                FileUrl = $"/uploads/{fileName}",
                FileName = file.FileName
            });
        }

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Submission uploaded successfully",
            submissionId = submission.SubmissionId,
            attemptNumber = submission.AttemptNumber
        });
    }

    [HttpGet("by-student-assignment")]
    public async Task<IActionResult> GetSubmissionByAssignmentAndStudent(int assignmentId, int studentId)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId && s.StudentId == studentId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();

        if (submission == null)
        {
            return NotFound("No submission found for this assignment and student.");
        }

        return Ok(submission);
    }
}
