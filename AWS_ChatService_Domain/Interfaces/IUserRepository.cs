using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByGoogleIdAsync(string googleId);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid userId);
    Task<bool> HasMessagesAsync(Guid userId);
}