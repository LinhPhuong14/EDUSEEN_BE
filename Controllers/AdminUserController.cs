using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Services;
using Sep490_Eduseen_BE.Dtos.User;
using Sep490_Eduseen_BE.Dtos;
using System;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Controllers
{
    [Route("api/admin/user")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(IUserService userService, ILogger<AdminUserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersForAdmin(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin requesting all users");
            var response = await _userService.GetAllUsersAsync(cancellationToken);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }
            return Ok(response.Data);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin requesting user statistics");
            var response = await _userService.GetUserStatisticsAsync(cancellationToken);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }
            return Ok(response.Data);
        }





        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            _logger.LogInformation("Admin updating user ID: {UserId}", id);
            var response = await _userService.UpdateUserAsync(id, updateUserDto);
            if (!response.Success)
            {
                return response.StatusCode == 404
                    ? NotFound(new { Error = response.ErrorMessage })
                    : StatusCode(response.StatusCode, new { Error = response.ErrorMessage });
            }
            return Ok(new { Message = "User updated successfully." });
        }


    }




} 