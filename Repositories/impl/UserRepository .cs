using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;

namespace Sep490_Eduseen_BE.Repositories.impl
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepositoryAsync<User> _userRepository;
        private readonly IRepositoryAsync<Role> _roleRepository;
        private readonly Sep490EduseenContext _context;

        public UserRepository(IRepositoryAsync<User> userRepository, IRepositoryAsync<Role> roleRepository, Sep490EduseenContext context)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AssignRoleAsync(int id, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return false;
            }

            var roleEntity = await GetRoleByNameAsync(role, cancellationToken);
            if (roleEntity == null)
            {
                return false;
            }

            user.RoleId = roleEntity.RoleId;
            await _userRepository.UpdateAsync(user, cancellationToken);
            return true;
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync(cancellationToken);
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<User>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _userRepository.AddAsync(user, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _userRepository.DeleteAsync(id, cancellationToken);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(password))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            }

            var roles = await _context.Roles
                .ToListAsync(cancellationToken);
            return roles.FirstOrDefault(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }
        public async Task CreatePasswordResetTokenAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken> GetPasswordResetTokenAsync(string tokenValue)
        {
            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == tokenValue);
        }

        public async Task DeletePasswordResetTokenAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetAllUsersWithRoleAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role) 
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdWithRoleAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
        }

    }
}