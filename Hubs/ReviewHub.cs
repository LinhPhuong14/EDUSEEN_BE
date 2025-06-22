using Microsoft.AspNetCore.SignalR;

namespace Sep490_Eduseen_BE.Hubs
{
    public class ReviewHub : Hub
    {
        public async Task SendReviewResponse(int courseId, int reviewId, object responseDto)
        {
            await Clients.Group($"course_{courseId}")
                .SendAsync("ReceiveReviewResponse", reviewId, responseDto);
        }

        public async Task JoinCourseGroup(int courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course_{courseId}");
        }

        public async Task LeaveCourseGroup(int courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course_{courseId}");
        }
    }
}
