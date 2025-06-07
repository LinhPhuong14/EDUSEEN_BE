using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserContext userContext, IUserRepository userRepository)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ServiceResponse<ProfileDTO>> GetProfileAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _userContext.GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user == null)
                {
                    return new ServiceResponse<ProfileDTO>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                return new ServiceResponse<ProfileDTO>
                {
                    Success = true,
                    Data = new ProfileDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AvatarUrl = user.AvatarUrl,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.RoleName
                    }
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ServiceResponse<ProfileDTO>
                {
                    Success = false,
                    StatusCode = 401,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ProfileDTO>
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving the profile."
                };
            }
        }

        public Task<ServiceResponse<ProfileDTO>> GetProfileAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> UpdateProfileAsync(UpdateProfileDTO profileDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _userContext.GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                user.FirstName = profileDto.FirstName ?? user.FirstName;
                user.LastName = profileDto.LastName ?? user.LastName;
                user.AvatarUrl = profileDto.AvatarUrl ?? user.AvatarUrl;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user, cancellationToken);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = 401,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while updating the profile."
                };
            }
        }

       
    }
}