using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.User;
using System.Threading;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<bool>> UpdateUserAsync(int id, UpdateUserDTO updateUserDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> ActivateUserAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DeactivateUserAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<UserDetailDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<UserDetailDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<UserStatisticsDto>> GetUserStatisticsAsync(int? year = null, CancellationToken cancellationToken = default);
    }
}