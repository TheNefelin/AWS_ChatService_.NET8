using AWS_ChatService_Domain.Entities;

namespace AWS_ChatService_Domain.Interfaces;

public interface IChatRoomRepository
{
    Task<IEnumerable<ChatRoom>> GetAllChatRoomsAsync();
    Task<ChatRoom?> GetChatRoomByIdAsync(Guid chatRoomId);
    Task CreateChatRoomAsync(ChatRoom chatRoom);
}