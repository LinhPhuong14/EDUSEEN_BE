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
    }
}