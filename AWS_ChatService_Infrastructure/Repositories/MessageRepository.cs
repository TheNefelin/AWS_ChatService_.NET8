using AWS_ChatService_Domain.Entities;
using AWS_ChatService_Domain.Interfaces;
using AWS_ChatService_Infrastructure.Configuration;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AWS_ChatService_Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ILogger<MessageRepository> _logger;
    private readonly DapperConnectionFactory _connectionFactory;

    public MessageRepository(ILogger<MessageRepository> logger, DapperConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatRoomAsync(Guid chatRoomId)
    {
        try
        {
            _logger.LogInformation("[MessageRepository] - Fetching messages for chat room {ChatRoomId}", chatRoomId);

            const string query = @"
            SELECT * FROM Messages 
            WHERE ChatRoomId = @ChatRoomId 
            ORDER BY SentAt ASC";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Message>(query, new { ChatRoomId = chatRoomId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MessageRepository] - Error fetching messages for chat room {ChatRoomId}", chatRoomId);
            throw; // Re-throw to be handled by the Application layer
        }
    }

    public async Task SendMessageAsync(Message message)
    {
        try
        {
            _logger.LogInformation("[MessageRepository] - Creating message with ID {MessageId} for chat room {ChatRoomId}", message.Id, message.ChatRoomId);

            const string sql = @"
            INSERT INTO Messages (Id, Content, SentAt, UserId, ChatRoomId)
            VALUES (@Id, @Content, @SentAt, @UserId, @ChatRoomId)";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MessageRepository] - Error creating message with ID {MessageId}", message.Id);
            throw;
        }
    }
}