using System.Threading.Tasks;
using Sep490_Eduseen_BE.Dtos.Auth;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface IOtpService
    {
        Task SaveOtpAsync(string email, string otp);
        Task<bool> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO);
    }
}