using AWS_ChatService_Application.DTOs;
using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Application.Mappers;

public class ChatRoomMapper
{
    public static ChatRoomDto ToDto(ChatRoom chatRoom) => new ChatRoomDto
    {
        Id = chatRoom.Id,
        Name = chatRoom.Name,
        CreatedAt = chatRoom.CreatedAt
    };
    public static ChatRoom ToEntity(ChatRoomDto dto) => new ChatRoom
    {
        Id = dto.Id,
        Name = dto.Name,
        CreatedAt = dto.CreatedAt
    };
    public static ChatRoom ToEntity(CreateChatRoomDto dto) => new ChatRoom
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        CreatedAt = DateTime.UtcNow
    };
}