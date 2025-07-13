using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Services;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //[HttpGet]
        //public async Task<IActionResult> ViewProfileAsync()
        //{
        //    _logger.LogInformation("Processing profile view request for user: {UserId}", User.Identity?.Name ?? "Unknown");

        //    var response = await _profileService.GetProfileAsync();
        //    if (!response.Success)
        //    {
        //        _logger.LogWarning("Profile view failed for user: {UserId}. Reason: {ErrorMessage}", User.Identity?.Name ?? "Unknown", response.ErrorMessage);
        //        return response.StatusCode == 404
        //            ? NotFound(new { Error = response.ErrorMessage })
        //            : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
        //    }

        //    _logger.LogInformation("Profile view successful for user: {UserId}", User.Identity?.Name ?? "Unknown");
        //    return Ok(new { Message = "Profile retrieved successfully.", Data = response.Data });
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileDTO profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request data.", Details = ModelState });
            }

            _logger.LogInformation("Processing profile update request for user: {UserId}", User.Identity?.Name ?? "Unknown");

            var response = await _profileService.UpdateProfileAsync(profileDto);
            if (!response.Success)
            {
                _logger.LogWarning("Profile update failed for user: {UserId}. Reason: {ErrorMessage}", User.Identity?.Name ?? "Unknown", response.ErrorMessage);
                return response.StatusCode == 404
                    ? NotFound(new { Error = response.ErrorMessage })
                    : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Profile update successful for user: {UserId}", User.Identity?.Name ?? "Unknown");
            return Ok(new { Message = "Profile updated successfully." });
        }
    }
}