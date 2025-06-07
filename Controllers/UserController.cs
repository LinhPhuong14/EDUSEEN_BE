using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Services;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request data for update user ID: {UserId}", id);
                return BadRequest(new { Error = "Invalid request data.", Details = ModelState });
            }

            _logger.LogInformation("Processing update user request for user ID: {UserId} by admin: {AdminId}", id, User.Identity?.Name ?? "Unknown");

            var response = await _userService.UpdateUserAsync(id, updateUserDto);
            if (!response.Success)
            {
                _logger.LogWarning("Update user failed for user ID: {UserId}. Reason: {ErrorMessage}", id, response.ErrorMessage);
                return response.StatusCode == 404
                    ? NotFound(new { Error = response.ErrorMessage })
                    : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Update user successful for user ID: {UserId}", id);
            return Ok(new { Message = "User updated successfully." });
        }

        [HttpPatch("{id}/active")]
        public async Task<IActionResult> ActivateUserAsync(int id)
        {
            _logger.LogInformation("Processing activate user request for user ID: {UserId} by admin: {AdminId}", id, User.Identity?.Name ?? "Unknown");

            var response = await _userService.ActivateUserAsync(id);
            if (!response.Success)
            {
                _logger.LogWarning("Activate user failed for user ID: {UserId}. Reason: {ErrorMessage}", id, response.ErrorMessage);
                return response.StatusCode == 404
                    ? NotFound(new { Error = response.ErrorMessage })
                    : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Activate user successful for user ID: {UserId}", id);
            return Ok(new { Message = "User activated successfully." });
        }

        [HttpPatch("{id}/deactive")]
        public async Task<IActionResult> DeactivateUserAsync(int id)
        {
            _logger.LogInformation("Processing deactivate user request for user ID: {UserId} by admin: {AdminId}", id, User.Identity?.Name ?? "Unknown");

            var response = await _userService.DeactivateUserAsync(id);
            if (!response.Success)
            {
                _logger.LogWarning("Deactivate user failed for user ID: {UserId}. Reason: {ErrorMessage}", id, response.ErrorMessage);
                return response.StatusCode == 404
                    ? NotFound(new { Error = response.ErrorMessage })
                    : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Deactivate user successful for user ID: {UserId}", id);
            return Ok(new { Message = "User deactivated successfully." });
        }
    }
}