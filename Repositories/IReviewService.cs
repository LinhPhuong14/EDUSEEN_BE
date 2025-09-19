using Sep490_Eduseen_BE.Dtos;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface IReviewService
    {
        Task<ReviewResponseDTO> RespondToReviewAsync(int reviewId, int teacherId, string responseText);
        Task<ReviewResponseDTO> UpdateReviewResponseAsync(int responseId, int teacherId, string responseText);
        Task<bool> DeleteReviewResponseAsync(int responseId, int teacherId);
    }

}
