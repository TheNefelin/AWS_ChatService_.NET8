using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using AWS_ChatService_Infrastructure.Configuration;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Infrastructure.Repositories;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;
    private readonly DapperConnectionFactory _dapperConnectionFactory;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger, DapperConnectionFactory dapperConnectionFactory)
    {
        _logger = logger;
        _dapperConnectionFactory = dapperConnectionFactory;
    }

    public async Task<IEnumerable<ChatRoom>> GetAllChatRoomsAsync()
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Fetching all chat rooms");

            const string sql = "SELECT * FROM ChatRooms ORDER BY CreatedAt DESC";
            using var connection = _dapperConnectionFactory.CreateConnection();
            return await connection.QueryAsync<ChatRoom>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error fetching all chat rooms");
            throw; // Re-throw to be handled by the Application layer
        }
    }

    public async Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId)
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Fetching chat room with ID {ChatRoomId}", chatRoomId);

            const string sql = "SELECT * FROM ChatRooms WHERE Id = @ChatRoomId";
            using var connection = _dapperConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ChatRoom>(sql, new { ChatRoomId = chatRoomId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error fetching chat room with ID {ChatRoomId}", chatRoomId);
            throw;
        }
    }

    public async Task CreateChatRoomAsync(ChatRoom room)
    {
        try
        {
            _logger.LogInformation("[ChatRoomRepository] - Creating chat room with ID {ChatRoomId}", room.Id);

            const string sql = @"INSERT INTO ChatRooms (Id, Name, CreatedAt) VALUES (@Id, @Name, @CreatedAt)";
            using var conn = _dapperConnectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChatRoomRepository] - Error creating chat room with ID {ChatRoomId}", room.Id);
            throw;
        }
    }
}