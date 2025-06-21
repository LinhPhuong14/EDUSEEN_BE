using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly Sep490EduseenContext _context;

    public AssignmentsController(Sep490EduseenContext context)
    {
        _context = context;
    }

    [HttpGet("{assignmentId}")]
    public async Task<IActionResult> GetAssignmentDetail(int assignmentId, [FromQuery] int studentId)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Submissions)
            .Include(a => a.CreatedByNavigation) // lấy thông tin người tạo
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

        if (assignment == null)
            return NotFound("Không tìm thấy bài tập");

        var submission = assignment.Submissions
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefault();

        var dto = new AssignmentDetailDto
        {
            AssignmentId = assignment.AssignmentId,
            Title = assignment.Title,
            Description = assignment.Description,
            DueDate = assignment.DueDate,
            CreatedByName = assignment.CreatedByNavigation.FirstName + " " + assignment.CreatedByNavigation.LastName,
            CreatedAt = assignment.CreatedAt,
            SubmissionStatus = submission == null ? "Chưa nộp" :
                (submission.Grade.HasValue ? "Đã chấm điểm" : "Đã nộp"),
            SubmittedAt = submission?.SubmittedAt,
            Grade = submission?.Grade
        };

        return Ok(dto);
    }

    [HttpGet("student/{studentId}/assignments")]
    public async Task<IActionResult> GetAssignmentsForStudent(int studentId)
    {
        var assignments = await _context.Assignments
            .Include(a => a.Submissions)
            .Include(a => a.Course)
            .OrderByDescending(a => a.DueDate)
            .Select(a => new
            {
                a.AssignmentId,
                a.Title,
                a.Description,
                a.DueDate,
                CourseTitle = a.Course.Title,
                IsSubmitted = a.Submissions.Any(s => s.StudentId == studentId)
            })
            .ToListAsync();

        return Ok(assignments);
    }

}
