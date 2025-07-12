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

        public async Task<ServiceResponse<bool>> UpdateUserAsync(int id, UpdateUserDTO UpdateUserDTO, CancellationToken cancellationToken = default)
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
                        ErrorMessage = "Admins cannot update their own role via this endpoint."
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

                // Validate chỉ được phép cập nhật role
                if (!UpdateUserDTO.RoleId.HasValue)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = "RoleId is required."
                    };
                }

                // Chỉ cập nhật role
                user.RoleId = UpdateUserDTO.RoleId.Value;
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
                    ErrorMessage = "An error occurred while updating the user role."
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
        public async Task<ServiceResponse<IEnumerable<UserListDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepository.GetAllUsersWithRoleAsync(cancellationToken);

                var userDtos = users.Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    RoleName = u.Role?.RoleName ?? "Unknown" // Lấy tên role
                }).ToList();

                return new ServiceResponse<IEnumerable<UserListDto>>
                {
                    Success = true,
                    Data = userDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<UserListDto>>
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving users."
                };
            }
        }

        public async Task<ServiceResponse<UserDetailDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userRepository.GetUserByIdWithRoleAsync(id, cancellationToken);
                if (user == null)
                {
                    return new ServiceResponse<UserDetailDto>
                    {
                        Success = false,
                        StatusCode = 404,
                        ErrorMessage = "User not found."
                    };
                }

                var userDto = new UserDetailDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    RoleName = user.Role?.RoleName ?? "Unknown",
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    AvatarUrl = user.AvatarUrl
                };

                return new ServiceResponse<UserDetailDto>
                {
                    Success = true,
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserDetailDto>
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving the user."
                };
            }
        }

    }
}