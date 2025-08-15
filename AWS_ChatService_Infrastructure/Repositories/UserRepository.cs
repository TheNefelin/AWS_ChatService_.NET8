using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using AWS_ChatService_Infrastructure.Configuration;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly DapperConnectionFactory _dapperConnectionFactory;

    public UserRepository(ILogger<UserRepository> logger, DapperConnectionFactory dapperConnectionFactory)
    {
        _logger = logger;
        _dapperConnectionFactory = dapperConnectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("[UserRepository] - Fetching all users from the database");
            const string sql = @"
                SELECT 
                    Id,
                    Email,
                    GoogleId,
                    Picture,
                    Names,
                    IsActive,
                    ConnectedAt,
                    LastLoginAt
                FROM Users
                ORDER BY Email DESC";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryAsync<User>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching all users");
            throw; // re-lanza para que lo maneje la capa Application
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Fetching user by ID: {userId}");
            const string sql = "SELECT * FROM Users WHERE Id = @userId";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching user by ID");
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Fetching user by email: {email}");
            const string sql = "SELECT * FROM Users WHERE Email = @email";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching user by email");
            throw;
        }
    }

    public async Task<User?> GetUserByGoogleIdAsync(string googleId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Fetching user by Google ID: {googleId}");
            const string sql = "SELECT * FROM Users WHERE GoogleId = @googleId";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { googleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error fetching user by Google ID");
            throw;
        }
    }

    public async Task CreateUserAsync(User user)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Creating user: {user.Email}");
            const string sql = @"INSERT INTO Users (Id, Username, ConnectedAt) VALUES (@Id, @Username, @ConnectedAt)";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error creating user");
            throw;
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Updating user: {user.Email}");
            const string sql = @"UPDATE Users SET Username = @Username, ConnectedAt = @ConnectedAt WHERE Id = @Id";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error updating user");
            throw;
        }
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Deleting user with ID: {userId}");
            const string query = @"DELETE FROM Users WHERE Id = @userId";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(query, new { userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error deleting user");
            throw;
        }
    }

    public async Task<bool> HasMessagesAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation($"[UserRepository] - Checking if user with ID {userId} has messages");
            const string sql = @"SELECT 1 FROM Messages WHERE UserId = @userId LIMIT 1";
            using var conn = _dapperConnectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(sql, new { userId }) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository] - Error checking if user has messages");
            throw;
        }
    }
}