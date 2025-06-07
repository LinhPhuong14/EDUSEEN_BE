using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;

        public UserService(IUserRepository userRepository, IUserContext userContext)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task<ServiceResponse<bool>> UpdateUserAsync(int id, UpdateUserDTO updateUserDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                if (currentUserId == id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "Admins cannot update their own account via this endpoint."
                    };
                }

                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                if (string.IsNullOrEmpty(updateUserDto.Username) && string.IsNullOrEmpty(updateUserDto.Email))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "Username or Email must be provided."
                    };
                }

                // Update fields
                user.Username = updateUserDto.Username ?? user.Username;
                user.Email = updateUserDto.Email ?? user.Email;
                user.FirstName = updateUserDto.FirstName ?? user.FirstName;
                user.LastName = updateUserDto.LastName ?? user.LastName;
                user.AvatarUrl = updateUserDto.AvatarUrl ?? user.AvatarUrl;
                user.RoleId = updateUserDto.RoleId ?? user.RoleId;
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
                    ErrorMessage = "An error occurred while updating the user."
                };
            }
        }

        public async Task<ServiceResponse<bool>> ActivateUserAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                if (currentUserId == id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "Admins cannot activate their own account via this endpoint."
                    };
                }

                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                if (user.IsActive == true)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "User is already active."
                    };
                }

                user.IsActive = true;
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
                    ErrorMessage = "An error occurred while activating the user."
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                if (currentUserId == id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "Admins cannot deactivate their own account via this endpoint."
                    };
                }

                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                if (user.IsActive == false)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "User is already deactivated."
                    };
                }

                user.IsActive = false;
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
                    ErrorMessage = "An error occurred while deactivating the user."
                };
            }
        }
    }
}