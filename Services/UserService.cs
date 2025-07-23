using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.User;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using System;
using System.Linq;
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
        public async Task<ServiceResponse<IEnumerable<UserDetailDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepository.GetAllUsersWithRoleAsync(cancellationToken);

                var userDtos = users.Select(u => new UserDetailDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    RoleName = u.Role?.RoleName ?? "Unknown",
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    AvatarUrl = u.AvatarUrl,
                    RoleId = u.RoleId,
                }).ToList();

                return new ServiceResponse<IEnumerable<UserDetailDto>>
                {
                    Success = true,
                    Data = userDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<UserDetailDto>>
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

        public async Task<ServiceResponse<UserStatisticsDto>> GetUserStatisticsAsync(int? year = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersWithRoleAsync(cancellationToken);
                
                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);

                var statistics = new UserStatisticsDto
                {
                    TotalUsers = allUsers.Count(),
                    ActiveUsers = allUsers.Count(u => u.IsActive == true),
                    InactiveUsers = allUsers.Count(u => u.IsActive == false),
                    Students = allUsers.Count(u => u.Role?.RoleName == "Student"),
                    Teachers = allUsers.Count(u => u.Role?.RoleName == "Teacher"),
                    Admins = allUsers.Count(u => u.Role?.RoleName == "Admin"),
                    NewUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth),
                    NewUsersThisWeek = allUsers.Count(u => u.CreatedAt >= startOfWeek),
                    AverageUsersPerDay = allUsers.Count() > 0 ? (double)allUsers.Count() / 30 : 0
                };

                // Users by role
                var roleGroups = allUsers.GroupBy(u => u.Role?.RoleName ?? "Unknown")
                    .Select(g => new UserRoleCountDto
                    {
                        RoleName = g.Key,
                        Count = g.Count(),
                        Percentage = allUsers.Count() > 0 ? (double)g.Count() / allUsers.Count() * 100 : 0
                    }).ToList();
                statistics.UsersByRole = roleGroups;

                // Users by status
                var statusGroups = allUsers.GroupBy(u => u.IsActive == true ? "Active" : "Inactive")
                    .Select(g => new UserStatusCountDto
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        Percentage = allUsers.Count() > 0 ? (double)g.Count() / allUsers.Count() * 100 : 0
                    }).ToList();
                statistics.UsersByStatus = statusGroups;

                // User registrations by month (đúng 12 tháng của năm truyền vào)
                int targetYear = year ?? now.Year;
                var months = Enumerable.Range(1, 12)
                    .Select(m => new { Year = targetYear, Month = m, MonthName = $"Th{m}" })
                    .ToList();

                var registrationsByMonth = months.Select(m => new UserRegistrationDto
                {
                    Month = m.MonthName,
                    Count = allUsers.Count(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Year == m.Year && u.CreatedAt.Value.Month == m.Month)
                }).ToList();
                statistics.UserRegistrationsByMonth = registrationsByMonth;

                return new ServiceResponse<UserStatisticsDto>
                {
                    Success = true,
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserStatisticsDto>
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorMessage = "An error occurred while retrieving user statistics."
                };
            }
        }





    }
}