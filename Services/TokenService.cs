using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sep490_Eduseen_BE.Exceptions;

namespace Sep490_Eduseen_BE.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRepositoryAsync<EmailConfirmationToken> _tokenRepository;

        public TokenService(
            ILogger<TokenService> logger,
            IConfiguration configuration,
            IUserRepository userRepository,
            IRepositoryAsync<EmailConfirmationToken> tokenRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));

            if (string.IsNullOrEmpty(_configuration["JWT:Key"]) ||
                string.IsNullOrEmpty(_configuration["JWT:Issuer"]) ||
                string.IsNullOrEmpty(_configuration["JWT:Audience"]))
            {
                throw new ArgumentException("JWT configuration values are missing.");
            }
        }

        public async Task<TokenDTO> CreateJWTTokenAsync(User user, bool populateExp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.RefreshToken = refreshToken;
            if (populateExp)
            {
                user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            }

            await _userRepository.UpdateAsync(user);

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDTO> RefreshJWTTokenAsync(TokenDTO tokenDTO)
        {
            if (tokenDTO == null || string.IsNullOrEmpty(tokenDTO.AccessToken) || string.IsNullOrEmpty(tokenDTO.RefreshToken))
            {
                throw new SecurityTokenException("Invalid token data.");
            }

            var principal = GetClaimsPrincipalFromExpiredToken(tokenDTO.AccessToken);
            var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                throw new SecurityTokenException("Email claim not found in token.");
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.RefreshToken != tokenDTO.RefreshToken || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                throw new RefreshTokenBadRequest();
            }

            return await CreateJWTTokenAsync(user, populateExp: false);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            var token = Guid.NewGuid().ToString();
            var confirmationToken = new EmailConfirmationToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _tokenRepository.AddAsync(confirmationToken);
            return token;
        }

        public async Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(int userId, string token)
        {
            if (userId <= 0 || string.IsNullOrEmpty(token))
            {
                return (false, new[] { "Invalid user ID or token." });
            }

            var tokens = await _tokenRepository.GetAsync();
            var confirmationToken = tokens.FirstOrDefault(t => t.UserId == userId && t.Token == token);

            if (confirmationToken == null || confirmationToken.ExpiresAt < DateTime.UtcNow)
            {
                return (false, new[] { "Invalid or expired token." });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, new[] { "User not found." });
            }

            await _userRepository.UpdateAsync(user);
            await _tokenRepository.DeleteAsync(confirmationToken.UserId);

            return (true, Array.Empty<string>());
        }

        public void SetTokenCookie(TokenDTO tokenDTO, HttpContext context)
        {
            if (tokenDTO == null || context == null)
            {
                _logger.LogWarning("TokenDTO or HttpContext is null in SetTokenCookie.");
                return;
            }

            context.Response.Cookies.Append("accessToken", tokenDTO.AccessToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

            context.Response.Cookies.Append("refreshToken", tokenDTO.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
        }

        public void DeleteTokenCookie(HttpContext context)
        {
            if (context == null)
            {
                _logger.LogWarning("HttpContext is null in DeleteTokenCookie.");
                return;
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };

            context.Response.Cookies.Delete("accessToken", cookieOptions);
            context.Response.Cookies.Delete("refreshToken", cookieOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            if (key.Length < 32)
            {
                throw new ArgumentException("JWT key must be at least 256 bits (32 bytes).");
            }
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Name, user.Username ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var role = await _userRepository.GetByIdAsync(user.UserId);
            if (role != null && role.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var expiryMinutes = double.TryParse(_configuration["JWT:ExpiryMinutes"], out var minutes) ? minutes : 5;
            return new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: signingCredentials
            );
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new SecurityTokenException("Token is null or empty.");
            }

            var jwtSettings = _configuration.GetSection("JWT");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                ValidateLifetime = false,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return principal;
        }
    }
}