using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Application.Mappers;

public class MessageMapper
{
    public static MessageDto ToDto(Message message) => new MessageDto
    {
        ChatRoomId = message.ChatRoomId,
        UserId = message.UserId,
        Content = message.Content,
    };

    public static Message ToEntity(MessageDto messageDto) => new Message
    {
        Id = Guid.NewGuid(),
        ChatRoomId = messageDto.ChatRoomId,
        UserId = messageDto.UserId,
        Content = messageDto.Content,
        SentAt = DateTime.UtcNow
    };
}