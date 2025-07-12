using Sep490_Eduseen_BE.Dtos.VideoCall;

namespace Sep490_Eduseen_BE.Services
{
    public interface IVideoCallService
    {
        Task<List<VideoCallHistoryDTO>> GetVideoCallHistoryForUser(int userId);
    }
}
