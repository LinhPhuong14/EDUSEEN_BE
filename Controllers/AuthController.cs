using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Services;
using System;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IOtpService otpService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request data.", Details = ModelState });
            }

            _logger.LogInformation("Processing login request for email: {Email}", loginDTO.Email);

            var response = await _authService.LoginAsync(loginDTO);
            if (!response.IsAuthSuccessful)
            {
                _logger.LogWarning("Login failed for email: {Email}. Reason: {ErrorMessage}", loginDTO.Email, response.ErrorMessage);
                return Unauthorized(new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Login successful for email: {Email}", loginDTO.Email);
            return Ok(new { Message = "Login successful.", Token = response.Token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request data.", Details = ModelState });
            }

            _logger.LogInformation("Processing registration request for email: {Email}", registerDTO.Email);

            var response = await _authService.RegisterAsync(registerDTO);
            if (!response.IsAuthSuccessful)
            {
                _logger.LogWarning("Registration failed for email: {Email}. Reason: {ErrorMessage}", registerDTO.Email, response.ErrorMessage);
                return BadRequest(new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("Registration OTP sent for email: {Email}", registerDTO.Email);
            return Ok(new { Message = "OTP sent to email.", Data = response });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] ConfirmOtpDTO confirmOtpDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request data.", Details = ModelState });
            }

            _logger.LogInformation("Processing OTP verification request for email: {Email}", confirmOtpDTO.Email);

            var response = await _authService.VerifyOtpAsync(confirmOtpDTO);
            if (!response.IsAuthSuccessful)
            {
                _logger.LogWarning("OTP verification failed for email: {Email}. Reason: {ErrorMessage}", confirmOtpDTO.Email, response.ErrorMessage);
                return BadRequest(new { Error = response.ErrorMessage });
            }

            _logger.LogInformation("OTP verified and user registered successfully for email: {Email}", confirmOtpDTO.Email);
            return Ok(new { Message = "OTP verified and user registered successfully.", Data = response });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            _logger.LogInformation("Processing logout request for user: {UserId}", User.Identity?.Name ?? "Unknown");

            await _authService.LogoutAsync();
            _logger.LogInformation("Logout successful for user: {UserId}", User.Identity?.Name ?? "Unknown");
            return Ok(new { Message = "Logged out successfully." });
        }
    }
}