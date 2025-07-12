using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/videocall")]
    [Authorize]
    public class VideoCallController : ControllerBase
    {
        private readonly IVideoCallService _videoCallService;

        public VideoCallController(IVideoCallService videoCallService)
        {
            _videoCallService = videoCallService;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetCallHistory(int userId)
        {
            var result = await _videoCallService.GetVideoCallHistoryForUser(userId);
            return Ok(result);
        }
    }
}
