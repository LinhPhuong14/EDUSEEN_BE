using Sep490_Eduseen_BE.Dtos;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface IReviewService
    {
        Task<ReviewResponseDTO> RespondToReviewAsync(int reviewId, int teacherId, string responseText);
    }

}
