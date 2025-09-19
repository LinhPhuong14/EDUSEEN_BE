using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Hubs;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;

public class ReviewService : IReviewService
{
    private readonly IHubContext<ReviewHub> _hubContext;
    private readonly Sep490EduseenContext _context;

    public ReviewService(IHubContext<ReviewHub> hubContext, Sep490EduseenContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    public async Task<ReviewResponseDTO> RespondToReviewAsync(int reviewId, int teacherId, string responseText)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) throw new Exception("Review not found");

        var response = new ReviewResponse
        {
            ReviewId = reviewId,
            TeacherId = teacherId,
            ResponseText = responseText,
            CreatedAt = DateTime.UtcNow
        };
        _context.ReviewResponses.Add(response);
        await _context.SaveChangesAsync();

        var courseId = review.CourseId;

        var responseDto = new ReviewResponseDTO
        {
            ResponseId = response.ResponseId,
            ReviewId = response.ReviewId,
            TeacherId = response.TeacherId,
            ResponseText = response.ResponseText,
            CreatedAt = response.CreatedAt
        };

        await _hubContext.Clients.Group($"course_{courseId}")
            .SendAsync("ReceiveReviewResponse", reviewId, responseDto);

        return responseDto;
    }

    public async Task<ReviewResponseDTO> UpdateReviewResponseAsync(int responseId, int teacherId, string responseText)
    {
        var response = await _context.ReviewResponses
            .Include(r => r.Review)
            .FirstOrDefaultAsync(r => r.ResponseId == responseId && r.TeacherId == teacherId);
        
        if (response == null) throw new Exception("Review response not found or unauthorized");

        response.ResponseText = responseText;
        response.CreatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        var courseId = response.Review.CourseId;

        var responseDto = new ReviewResponseDTO
        {
            ResponseId = response.ResponseId,
            ReviewId = response.ReviewId,
            TeacherId = response.TeacherId,
            ResponseText = response.ResponseText,
            CreatedAt = response.CreatedAt
        };

        await _hubContext.Clients.Group($"course_{courseId}")
            .SendAsync("UpdateReviewResponse", responseId, responseDto);

        return responseDto;
    }

    public async Task<bool> DeleteReviewResponseAsync(int responseId, int teacherId)
    {
        var response = await _context.ReviewResponses
            .Include(r => r.Review)
            .FirstOrDefaultAsync(r => r.ResponseId == responseId && r.TeacherId == teacherId);
        
        if (response == null) return false;

        var courseId = response.Review.CourseId;
        var reviewId = response.ReviewId;

        _context.ReviewResponses.Remove(response);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group($"course_{courseId}")
            .SendAsync("DeleteReviewResponse", responseId, reviewId);

        return true;
    }
}
