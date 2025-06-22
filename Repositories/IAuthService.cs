using System.Threading.Tasks;
using Sep490_Eduseen_BE.Dtos.Auth;

namespace Sep490_Eduseen_BE.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResponseDTO> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO);
        Task LogoutAsync();
        Task<GenericResponseDTO> SendPasswordResetAsync(ForgotPasswordDTO dto);
        Task<GenericResponseDTO> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<GenericResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
    }
}