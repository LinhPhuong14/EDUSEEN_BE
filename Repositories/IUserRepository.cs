using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface IUserRepository
    {
        Task<bool> AssignRoleAsync(int id, string role, CancellationToken cancellationToken = default);
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(User user, string password, CancellationToken cancellationToken = default);
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task CreatePasswordResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken> GetPasswordResetTokenAsync(string token);
        Task DeletePasswordResetTokenAsync(PasswordResetToken token);
        Task<IEnumerable<User>> GetAllUsersWithRoleAsync(CancellationToken cancellationToken = default);
        Task<User?> GetUserByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);
    }
}