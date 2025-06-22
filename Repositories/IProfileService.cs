using Sep490_Eduseen_BE.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface IProfileService
    {
        Task<ServiceResponse<ProfileDTO>> GetProfileAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> UpdateProfileAsync(UpdateProfileDTO profileDto, CancellationToken cancellationToken = default);
    }
}