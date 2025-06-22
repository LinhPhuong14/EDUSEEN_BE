using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface ITokenService
    {
        Task<TokenDTO> CreateJWTTokenAsync(User user, bool populateExp);
        Task<TokenDTO> RefreshJWTTokenAsync(TokenDTO tokenDTO);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
        Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(int userId, string token);
        void SetTokenCookie(TokenDTO tokenDTO, HttpContext context);
        void DeleteTokenCookie(HttpContext context);
    }
}