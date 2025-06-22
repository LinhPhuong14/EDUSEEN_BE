using System.Security.Claims;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;

namespace Sep490_Eduseen_BE.Repositories.impl;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepositoryAsync<User> _userRepository;

    public UserContext(IHttpContextAccessor httpContextAccessor, IRepositoryAsync<User> userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public int GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        return userId;
    }

    public async Task<int> GetCurrentUserIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        var dbUser = await _userRepository.GetByIdAsync(userId);

        if (dbUser == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        return userId;
    }
}