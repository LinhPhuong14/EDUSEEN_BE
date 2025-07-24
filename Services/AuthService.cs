using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Sep490_Eduseen_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IOtpService otpService,
            IEmailService emailService,
            IMemoryCache cache,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return new AuthResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Email và mật khẩu là bắt buộc."
                };
            }

            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDTO.Password))
            {
                return new AuthResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Email hoặc mật khẩu không hợp lệ."
                };
            }

            if (!user.IsActive.GetValueOrDefault())
            {
                return new AuthResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Tài khoản không hoạt động."
                };
            }

            var tokenDTO = await _tokenService.CreateJWTTokenAsync(user, populateExp: true);
            _tokenService.SetTokenCookie(tokenDTO, _httpContextAccessor.HttpContext);

            // Chuẩn bị thông tin người dùng cơ bản để trả về cho FE
            var userInfo = new Sep490_Eduseen_BE.Dtos.ProfileDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName
            };

            return new AuthResponseDTO
            {
                IsAuthSuccessful = true,
                Token = tokenDTO,
                User = userInfo
            };
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            // Kiểm tra đầu vào
            if (registerDTO == null)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Dữ liệu đăng ký không hợp lệ." };
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Email là bắt buộc." };
            }
            if (string.IsNullOrWhiteSpace(registerDTO.UserName))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Tên người dùng là bắt buộc." };
            }
            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Mật khẩu là bắt buộc." };
            }

            // Validate Email
            if (registerDTO.Email.Length > 255)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Email không được dài quá 255 ký tự." };
            }
            if (!IsValidEmail(registerDTO.Email))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Email không đúng định dạng." };
            }

            // Validate UserName
            if (registerDTO.UserName.Length < 3 || registerDTO.UserName.Length > 50)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Tên người dùng phải từ 3 đến 50 ký tự." };
            }
            if (!Regex.IsMatch(registerDTO.UserName, @"^[a-zA-Z0-9_]+$"))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Tên người dùng chỉ được chứa chữ cái, số và dấu gạch dưới." };
            }

            // Validate Password
            if (registerDTO.Password.Length < 8)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Mật khẩu phải dài ít nhất 8 ký tự." };
            }
            if (registerDTO.Password.Length > 128)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Mật khẩu không được dài quá 128 ký tự." };
            }
            if (!Regex.IsMatch(registerDTO.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ cái in hoa, 1 chữ cái thường, 1 số và 1 ký tự đặc biệt (@$!%*?&)." };
            }

            // Kiểm tra email đã tồn tại
            var existingUser = await _userRepository.GetByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Email đã tồn tại." };
            }

            // Kiểm tra username đã tồn tại
            var existingUserName = await _userRepository.GetByUserNameAsync(registerDTO.UserName);
            if (existingUserName != null)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Tên người dùng đã tồn tại." };
            }

            // Tạo OTP và lưu vào DB
            var otp = new Random().Next(100000, 999999).ToString();
            await _otpService.SaveOtpAsync(registerDTO.Email, otp);

            // Lưu RegisterDTO trong cache để xác minh OTP
            var cacheKey = $"RegisterDTO_{registerDTO.Email}";
            _cache.Set(cacheKey, registerDTO, TimeSpan.FromMinutes(10)); // Cache trong 10 phút

            // Gửi OTP qua email
            try
            {
                await _emailService.SendEmailAsync(
                    registerDTO.Email,
                    "Xác minh OTP",
                    $"<h3>Mã OTP của bạn là: {otp}</h3><p>Vui lòng nhập mã này để hoàn tất đăng ký.</p>"
                );

                _logger.LogInformation("OTP đã được gửi cho email: {Email}", registerDTO.Email);

                return new AuthResponseDTO
                {
                    IsAuthSuccessful = true,
                    ErrorMessage = "OTP đã được gửi đến email của bạn. Vui lòng xác minh để hoàn tất đăng ký."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gửi OTP thất bại cho email: {Email}", registerDTO.Email);
                _cache.Remove(cacheKey); // Xóa dữ liệu cache khi thất bại
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Gửi OTP thất bại. Vui lòng thử lại." };
            }
        }

        public async Task<AuthResponseDTO> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO)
        {
            if (confirmOtpDTO == null || string.IsNullOrEmpty(confirmOtpDTO.Email) || string.IsNullOrEmpty(confirmOtpDTO.Otp))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Email và OTP là bắt buộc." };
            }

            // Xác minh OTP sử dụng IOtpService
            var isOtpValid = await _otpService.VerifyOtpAsync(confirmOtpDTO);
            if (!isOtpValid)
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "OTP không hợp lệ hoặc đã hết hạn." };
            }

            // Lấy RegisterDTO từ cache
            var cacheKey = $"RegisterDTO_{confirmOtpDTO.Email}";
            if (!_cache.TryGetValue(cacheKey, out RegisterDTO registerDTO))
            {
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Dữ liệu đăng ký đã hết hạn hoặc không tìm thấy. Vui lòng đăng ký lại." };
            }

            // Lấy vai trò "User"
            var role = await _userRepository.GetRoleByNameAsync("User");
            if (role == null)
            {
                _logger.LogError("Vai trò User không tìm thấy trong cơ sở dữ liệu.");
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Vai trò User không tìm thấy trong hệ thống. Vui lòng liên hệ hỗ trợ." };
            }

            _logger.LogInformation("Đã tìm thấy vai trò User với RoleId: {RoleId}", role.RoleId);

            // Tạo Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // Refresh token hết hạn sau 7 ngày

            // Tạo và lưu người dùng
            var user = new User
            {
                Email = confirmOtpDTO.Email,
                Username = registerDTO.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                RoleId = role.RoleId, 
                IsActive = true, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };

            _logger.LogInformation("Đang tạo người dùng với RoleId: {RoleId}, IsActive: {IsActive}, RefreshToken: {RefreshToken}, RefreshTokenExpiresAt: {RefreshTokenExpiresAt} cho email: {Email}",
                user.RoleId, user.IsActive, user.RefreshToken, user.RefreshTokenExpiresAt, user.Email);

            try
            {
                await _userRepository.CreateAsync(user, registerDTO.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tạo người dùng thất bại cho email: {Email}", confirmOtpDTO.Email);
                return new AuthResponseDTO { IsAuthSuccessful = false, ErrorMessage = "Tạo người dùng thất bại: " + ex.Message };
            }

            _cache.Remove(cacheKey); // Xóa dữ liệu cache sau khi đăng ký thành công
            _logger.LogInformation("Người dùng đã đăng ký thành công với vai trò User cho email: {Email}", confirmOtpDTO.Email);

            return new AuthResponseDTO
            {
                IsAuthSuccessful = true,
                ErrorMessage = "Đăng ký thành công!"
            };
        }

        public async Task LogoutAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("HttpContext là null trong LogoutAsync.");
                return;
            }

            var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Không tìm thấy người dùng đã xác thực trong LogoutAsync.");
                return;
            }

            var user = await _userRepository.GetByIdAsync(int.Parse(userId));
            if (user == null)
            {
                _logger.LogWarning("Không tìm thấy người dùng để logout với ID: {UserId}", userId);
                return;
            }

            _tokenService.DeleteTokenCookie(_httpContextAccessor.HttpContext);
            _logger.LogInformation("Người dùng đã logout thành công với ID: {UserId}", userId);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Sử dụng Regex để kiểm tra định dạng email
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public async Task<GenericResponseDTO> SendPasswordResetAsync(ForgotPasswordDTO dto)
        {
            // 1. Kiểm tra user theo email
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return new GenericResponseDTO { Success = false, Message = "No account found with this email." };

            // 2. Sinh token reset
            var token = Guid.NewGuid().ToString("N");
            var expiresAt = DateTime.UtcNow.AddMinutes(15);

            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = expiresAt
            };

            // Sử dụng repository để lưu token
            try
            {
                await _userRepository.CreatePasswordResetTokenAsync(resetToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating password reset token");
                return new GenericResponseDTO { Success = false, Message = "Error processing your request." };
            }

            // 3. Gửi email chứa link reset
            try
            {
                var resetLink = $"http://localhost:3000/auth/reset-password?token={token}";
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Password Reset",
                    $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>. This link expires in 15 minutes."
                );
                return new GenericResponseDTO { Success = true, Message = "Reset link sent. Please check your email." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset email");
                return new GenericResponseDTO { Success = false, Message = "We’re unable to process your request right now. Please try again later." };
            }
        }

        public async Task<GenericResponseDTO> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            // 1. Kiểm tra token hợp lệ
            var resetToken = await _userRepository.GetPasswordResetTokenAsync(dto.Token);
            if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                return new GenericResponseDTO { Success = false, Message = "This reset link has expired or is invalid. Please request a new one." };

            // 2. Kiểm tra mật khẩu mới
            if (dto.NewPassword != dto.ConfirmPassword)
                return new GenericResponseDTO { Success = false, Message = "New password and confirmation do not match." };
            if (dto.NewPassword.Length < 8)
                return new GenericResponseDTO { Success = false, Message = "Password must contain at least 8 characters." };

            // 3. Cập nhật mật khẩu
            var user = await _userRepository.GetByIdAsync(resetToken.UserId);
            if (user == null)
                return new GenericResponseDTO { Success = false, Message = "User not found." };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password");
                return new GenericResponseDTO { Success = false, Message = "Error updating password." };
            }

            // 4. Xóa token đã dùng
            try
            {
                await _userRepository.DeletePasswordResetTokenAsync(resetToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reset token");
                // Không return error vì password đã được reset thành công
            }

            return new GenericResponseDTO { Success = true, Message = "Your password has been reset. You may now log in." };
        }
        public async Task<GenericResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (dto == null)
                return new GenericResponseDTO { Success = false, Message = "Invalid data." };

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.ConfirmNewPassword))
                return new GenericResponseDTO { Success = false, Message = "All password fields are required." };

            // 2. Lấy user từ repository
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new GenericResponseDTO { Success = false, Message = "User not found." };

            // 3. Kiểm tra mật khẩu hiện tại
            var isCurrentPasswordValid = await _userRepository.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!isCurrentPasswordValid)
                return new GenericResponseDTO { Success = false, Message = "The current password you entered is incorrect." };

            // 4. Kiểm tra mật khẩu mới và xác nhận
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return new GenericResponseDTO { Success = false, Message = "New password and confirmation do not match." };

            if (dto.NewPassword.Length < 8)
                return new GenericResponseDTO { Success = false, Message = "Password must contain at least 8 characters." };

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
                return new GenericResponseDTO { Success = false, Message = "New password must be different from old password." };

            // 5. (Có thể thêm kiểm tra độ mạnh mật khẩu nếu muốn)

            // 6. Hash và cập nhật mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password");
                return new GenericResponseDTO { Success = false, Message = "We couldn't update your password. Please try again later." };
            }

            // 7. (Có thể gửi email thông báo đổi mật khẩu nếu muốn)

            return new GenericResponseDTO { Success = true, Message = "Your password has been changed successfully." };
        }




    }
}