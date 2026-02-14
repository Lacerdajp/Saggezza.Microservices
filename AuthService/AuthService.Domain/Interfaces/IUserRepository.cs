using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetLockedUsersAsync();
    Task<List<User>> GetActiveUsersAsync();
    Task<List<User>> GetInactiveUsersAsync();
}